using NUnit.Framework;
using SlimeExperiment.Battle;
using SlimeExperiment.Core;
using SlimeExperiment.Data;

namespace SlimeExperiment.Tests.EditMode
{
    public sealed class PrototypeBattleTests
    {
        [Test]
        public void DraftAssemblyUsesSelectedCardValues()
        {
            PrototypeContentLibrary library = new PrototypeContentLibrary();
            DraftService draftService = new DraftService(library, new FixedRandomSource());

            ExperimentBuildResult result = draftService.Assemble(
                library.AttackCards[0],
                library.UtilityCards[0],
                library.AttributeCards[1]);

            Assert.AreEqual(0, result.MinAttack);
            Assert.AreEqual(10, result.MaxAttack);
            Assert.AreEqual(UtilityEffectType.BonusAttackChance, result.UtilityEffectType);
            Assert.AreEqual(AttributeType.Fire, result.AttributeType);
        }

        [Test]
        public void MonsterFactoryUsesStageRuleValues()
        {
            PrototypeContentLibrary library = new PrototypeContentLibrary();
            MonsterFactory factory = new MonsterFactory(library, new FixedRandomSource());

            MonsterEncounter encounter = factory.CreateEncounter(3);

            Assert.AreEqual(18, encounter.Rule.Health);
            Assert.AreEqual(3, encounter.Rule.MinAttack);
            Assert.AreEqual(6, encounter.Rule.MaxAttack);
        }

        [Test]
        public void RelicPoolBlocksMutuallyExclusiveRelics()
        {
            PrototypeContentLibrary library = new PrototypeContentLibrary();
            RelicPoolService poolService = new RelicPoolService(library);

            RelicDefinition skateboard = FindRelic(library, "relic_skateboard");
            System.Collections.Generic.List<RelicDefinition> owned = new System.Collections.Generic.List<RelicDefinition>
            {
                skateboard
            };

            System.Collections.Generic.List<RelicDefinition> eligible = poolService.GetEligibleRelics(owned);

            Assert.IsFalse(ContainsRelic(eligible, "relic_twin_cherry"));
        }

        [Test]
        public void DelayedKillEffectDefeatsMonsterOnNextTurnStart()
        {
            BattleSimulator simulator = new BattleSimulator(new FixedRandomSource());
            BattleUnitState player = new BattleUnitState
            {
                DisplayName = "player",
                IsPlayer = true,
                MaxHealth = 20,
                CurrentHealth = 20,
                MinAttack = 1,
                MaxAttack = 1,
                AttackFailureChance = 100f,
                AttackFailureCausesDelayedKill = true
            };
            BattleUnitState monster = new BattleUnitState
            {
                DisplayName = "monster",
                IsPlayer = false,
                MaxHealth = 10,
                CurrentHealth = 10,
                MinAttack = 0,
                MaxAttack = 0
            };

            BattleReport report = simulator.Simulate(player, monster, 1, false);

            Assert.AreEqual(BattleOutcome.Victory, report.Outcome);
            Assert.AreEqual(0, report.FinalMonsterState.CurrentHealth);
        }

        [Test]
        public void MirrorAndChanceBoostsAreCappedAtHundred()
        {
            PrototypeContentLibrary library = new PrototypeContentLibrary();
            ExperimentBuildResult build = new ExperimentBuildResult
            {
                AttackCard = library.AttackCards[5],
                UtilityCard = library.UtilityCards[0],
                AttributeCard = library.AttributeCards[0],
                MinAttack = 5,
                MaxAttack = 5,
                UtilityEffectType = UtilityEffectType.BonusAttackChance,
                UtilityChancePercent = 25f,
                AttributeType = AttributeType.Neutral
            };

            System.Collections.Generic.List<RelicDefinition> relics = new System.Collections.Generic.List<RelicDefinition>
            {
                FindRelic(library, "relic_clockwork"),
                FindRelic(library, "relic_stardust"),
                FindRelic(library, "relic_mirror")
            };

            BattleUnitState state = RelicEffectResolver.BuildPlayerState(build, relics, 20, 0, false);

            Assert.AreEqual(100f, state.ExtraAttackChance);
        }

        private static RelicDefinition FindRelic(PrototypeContentLibrary library, string relicId)
        {
            for (int index = 0; index < library.Relics.Count; index++)
            {
                if (library.Relics[index].Id == relicId)
                {
                    return library.Relics[index];
                }
            }

            return null;
        }

        private static bool ContainsRelic(System.Collections.Generic.List<RelicDefinition> relics, string relicId)
        {
            for (int index = 0; index < relics.Count; index++)
            {
                if (relics[index].Id == relicId)
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class FixedRandomSource : IRandomSource
        {
            public int NextInclusive(int minValue, int maxValue)
            {
                return minValue;
            }

            public float NextPercent()
            {
                return 99f;
            }
        }
    }
}
