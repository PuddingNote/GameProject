using System.Collections.Generic;
using SlimeExperiment.Battle;
using SlimeExperiment.Data;

namespace SlimeExperiment.Core
{
    public sealed class GameSession
    {
        public int ExperimentNumber { get; private set; }
        public DraftPhase DraftPhase { get; private set; }
        public int StageIndex { get; private set; }
        public int PersistentCurrentHealth { get; set; }
        public int PiggyBankStacks { get; set; }
        public CardDefinition SelectedAttackCard { get; private set; }
        public CardDefinition SelectedUtilityCard { get; private set; }
        public CardDefinition SelectedAttributeCard { get; private set; }
        public ExperimentBuildResult BuildResult { get; private set; }
        public MonsterEncounter CurrentEncounter { get; set; }
        public BattleReport LastBattleReport { get; set; }
        public RelicDefinition LastRewardRelic { get; set; }
        public bool RunFinished { get; set; }
        public bool RunSucceeded { get; set; }

        private readonly List<RelicDefinition> ownedRelics = new List<RelicDefinition>();

        public IReadOnlyList<RelicDefinition> OwnedRelics => ownedRelics;

        public void Begin(int experimentNumber)
        {
            ExperimentNumber = experimentNumber;
            DraftPhase = DraftPhase.Attack;
            StageIndex = 1;
            PersistentCurrentHealth = 20;
            PiggyBankStacks = 0;
            SelectedAttackCard = null;
            SelectedUtilityCard = null;
            SelectedAttributeCard = null;
            BuildResult = null;
            CurrentEncounter = null;
            LastBattleReport = null;
            LastRewardRelic = null;
            RunFinished = false;
            RunSucceeded = false;
            ownedRelics.Clear();
        }

        public void ApplySelectedCard(CardDefinition selectedCard, DraftService draftService)
        {
            if (DraftPhase == DraftPhase.Attack)
            {
                SelectedAttackCard = selectedCard;
                DraftPhase = DraftPhase.Utility;
                return;
            }

            if (DraftPhase == DraftPhase.Utility)
            {
                SelectedUtilityCard = selectedCard;
                DraftPhase = DraftPhase.Attribute;
                return;
            }

            if (DraftPhase == DraftPhase.Attribute)
            {
                SelectedAttributeCard = selectedCard;
                BuildResult = draftService.Assemble(SelectedAttackCard, SelectedUtilityCard, SelectedAttributeCard);
                DraftPhase = DraftPhase.Complete;
            }
        }

        public void AddRelic(RelicDefinition relicDefinition)
        {
            ownedRelics.Add(relicDefinition);
            LastRewardRelic = relicDefinition;
        }

        public bool IsBossStage()
        {
            return StageIndex >= 6;
        }

        public void MoveToNextStage()
        {
            StageIndex++;
        }

        public List<string> GetSelectedCardNames()
        {
            List<string> names = new List<string>();
            if (SelectedAttackCard != null)
            {
                names.Add(SelectedAttackCard.DisplayName);
            }

            if (SelectedUtilityCard != null)
            {
                names.Add(SelectedUtilityCard.DisplayName);
            }

            if (SelectedAttributeCard != null)
            {
                names.Add(SelectedAttributeCard.DisplayName);
            }

            return names;
        }

        public List<string> GetOwnedRelicNames()
        {
            List<string> names = new List<string>();
            for (int index = 0; index < ownedRelics.Count; index++)
            {
                names.Add(ownedRelics[index].DisplayName);
            }

            return names;
        }
    }
}
