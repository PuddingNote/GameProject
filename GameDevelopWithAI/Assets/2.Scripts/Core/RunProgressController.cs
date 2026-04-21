using System;
using System.Collections.Generic;
using SlimeExperiment.Battle;
using SlimeExperiment.Data;
using SlimeExperiment.Logbook;

namespace SlimeExperiment.Core
{
    public sealed class RunProgressController
    {
        private readonly GameSession gameSession;
        private readonly MonsterFactory monsterFactory;
        private readonly BattleSimulator battleSimulator;
        private readonly RelicPoolService relicPoolService;
        private readonly ExperimentLogService experimentLogService;
        private readonly IRandomSource randomSource;

        public RunProgressController(
            GameSession gameSession,
            MonsterFactory monsterFactory,
            BattleSimulator battleSimulator,
            RelicPoolService relicPoolService,
            ExperimentLogService experimentLogService,
            IRandomSource randomSource)
        {
            this.gameSession = gameSession;
            this.monsterFactory = monsterFactory;
            this.battleSimulator = battleSimulator;
            this.relicPoolService = relicPoolService;
            this.experimentLogService = experimentLogService;
            this.randomSource = randomSource;
        }

        public MonsterEncounter PrepareEncounter()
        {
            if (HasPiggyBank())
            {
                gameSession.PiggyBankStacks++;
            }

            MonsterEncounter encounter = monsterFactory.CreateEncounter(gameSession.StageIndex);
            gameSession.CurrentEncounter = encounter;
            return encounter;
        }

        public BattleReport StartBattle()
        {
            BattleUnitState playerState = RelicEffectResolver.BuildPlayerState(
                gameSession.BuildResult,
                gameSession.OwnedRelics,
                gameSession.PersistentCurrentHealth,
                gameSession.PiggyBankStacks,
                gameSession.IsBossStage());

            BattleUnitState monsterState = monsterFactory.CreateBattleState(gameSession.CurrentEncounter);
            BattleReport report = battleSimulator.Simulate(playerState, monsterState, gameSession.StageIndex, gameSession.IsBossStage());
            gameSession.LastBattleReport = report;
            gameSession.PersistentCurrentHealth = report.FinalPlayerState.CurrentHealth;

            if (report.Outcome == BattleOutcome.Defeat)
            {
                FinalizeRun(false);
                return report;
            }

            if (gameSession.IsBossStage())
            {
                FinalizeRun(true);
            }

            return report;
        }

        public RelicDefinition GrantVictoryRelic()
        {
            List<RelicDefinition> candidates = relicPoolService.GetEligibleRelics(gameSession.OwnedRelics);
            if (candidates.Count == 0)
            {
                return null;
            }

            int index = randomSource.NextInclusive(0, candidates.Count - 1);
            RelicDefinition rewardedRelic = candidates[index];
            gameSession.AddRelic(rewardedRelic);
            experimentLogService.MarkRelicDiscovered(rewardedRelic.Id);
            return rewardedRelic;
        }

        public void AdvanceAfterReward()
        {
            gameSession.MoveToNextStage();
            gameSession.LastRewardRelic = null;
        }

        public void FinalizeRun(bool succeeded)
        {
            gameSession.RunFinished = true;
            gameSession.RunSucceeded = succeeded;

            ExperimentRecord record = new ExperimentRecord
            {
                ExperimentNumber = gameSession.ExperimentNumber,
                Succeeded = succeeded,
                ReachedStage = gameSession.StageIndex,
                SelectedCards = gameSession.GetSelectedCardNames(),
                SelectedCardEffects = GetSelectedCardEffects(),
                AcquiredRelics = gameSession.GetOwnedRelicNames(),
                AcquiredRelicIds = GetOwnedRelicIds(),
                ResultSummary = succeeded ? "보스 실험 성공" : "실험 실패",
                CreatedAt = DateTime.UtcNow.ToString("O")
            };

            experimentLogService.SaveRecord(record);
        }

        private bool HasPiggyBank()
        {
            IReadOnlyList<RelicDefinition> relics = gameSession.OwnedRelics;
            for (int index = 0; index < relics.Count; index++)
            {
                if (relics[index].Id == "relic_piggy_bank")
                {
                    return true;
                }
            }

            return false;
        }

        private List<string> GetOwnedRelicIds()
        {
            List<string> ids = new List<string>();
            IReadOnlyList<RelicDefinition> relics = gameSession.OwnedRelics;
            for (int index = 0; index < relics.Count; index++)
            {
                ids.Add(relics[index].Id);
            }

            return ids;
        }

        private List<string> GetSelectedCardEffects()
        {
            List<string> effects = new List<string>();
            if (gameSession.SelectedAttackCard != null)
            {
                effects.Add(gameSession.SelectedAttackCard.Description);
            }

            if (gameSession.SelectedUtilityCard != null)
            {
                effects.Add(gameSession.SelectedUtilityCard.Description);
            }

            if (gameSession.SelectedAttributeCard != null)
            {
                effects.Add(gameSession.SelectedAttributeCard.Description);
            }

            return effects;
        }
    }
}
