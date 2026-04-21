using System.Collections.Generic;
using SlimeExperiment.Core;
using SlimeExperiment.Data;

namespace SlimeExperiment.Battle
{
    public sealed class MonsterFactory
    {
        private readonly PrototypeContentLibrary contentLibrary;
        private readonly IRandomSource randomSource;

        public MonsterFactory(PrototypeContentLibrary contentLibrary, IRandomSource randomSource)
        {
            this.contentLibrary = contentLibrary;
            this.randomSource = randomSource;
        }

        public MonsterEncounter CreateEncounter(int stageIndex)
        {
            MonsterRuleDefinition rule = FindRule(stageIndex);
            AttributeType attributeType = RollAttribute();
            MonsterUtilityType utilityType = RollUtility(rule.IsBoss);

            string utilityLabel = GetMonsterUtilityLabel(utilityType, rule.IsBoss);
            string attributeLabel = GetAttributeLabel(attributeType);
            string prefix = rule.IsBoss ? "보스" : $"테스트 대상 {stageIndex}";

            return new MonsterEncounter
            {
                Rule = rule,
                AttributeType = attributeType,
                UtilityType = utilityType,
                DisplayName = $"{prefix} / {attributeLabel}",
                RouletteSummary = $"공격 {rule.MinAttack}~{rule.MaxAttack}\n유틸: {utilityLabel}\n속성: {attributeLabel}"
            };
        }

        public BattleUnitState CreateBattleState(MonsterEncounter encounter)
        {
            BattleUnitState state = new BattleUnitState
            {
                DisplayName = encounter.DisplayName,
                IsPlayer = false,
                IsBoss = encounter.Rule.IsBoss,
                AttributeType = encounter.AttributeType,
                MaxHealth = encounter.Rule.Health,
                CurrentHealth = encounter.Rule.Health,
                MinAttack = encounter.Rule.MinAttack,
                MaxAttack = encounter.Rule.MaxAttack,
                DamageReductionMultiplier = 0.5f
            };

            switch (encounter.UtilityType)
            {
                case MonsterUtilityType.ExtraAttack:
                    state.ExtraAttackChance = encounter.Rule.ExtraAttackChance;
                    break;
                case MonsterUtilityType.Dodge:
                    state.DodgeChance = encounter.Rule.DodgeChance;
                    break;
                case MonsterUtilityType.DamageReduction:
                    state.DamageReductionChance = encounter.Rule.DamageReductionChance;
                    break;
                case MonsterUtilityType.Execute:
                    state.ExecuteChance = encounter.Rule.ExecuteChance;
                    break;
            }

            return state;
        }

        private MonsterRuleDefinition FindRule(int stageIndex)
        {
            IReadOnlyList<MonsterRuleDefinition> rules = contentLibrary.MonsterRules;

            for (int index = 0; index < rules.Count; index++)
            {
                if (rules[index].StageIndex == stageIndex)
                {
                    return rules[index];
                }
            }

            return rules[rules.Count - 1];
        }

        private AttributeType RollAttribute()
        {
            int rolledIndex = randomSource.NextInclusive(0, 5);
            return (AttributeType)rolledIndex;
        }

        private MonsterUtilityType RollUtility(bool isBoss)
        {
            int rolledIndex = randomSource.NextInclusive(0, 4);
            return (MonsterUtilityType)rolledIndex;
        }

        public string GetMonsterUtilityLabel(MonsterUtilityType utilityType, bool isBoss)
        {
            switch (utilityType)
            {
                case MonsterUtilityType.ExtraAttack:
                    return isBoss ? "추가 공격 30%" : "추가 공격 20%";
                case MonsterUtilityType.Dodge:
                    return isBoss ? "공격 회피 30%" : "공격 회피 20%";
                case MonsterUtilityType.DamageReduction:
                    return isBoss ? "피해 감소 40%" : "피해 감소 30%";
                case MonsterUtilityType.Execute:
                    return isBoss ? "즉사 공격 15%" : "즉사 공격 5%";
                default:
                    return "효과 없음";
            }
        }

        public string GetAttributeLabel(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Fire:
                    return "불";
                case AttributeType.Water:
                    return "물";
                case AttributeType.Grass:
                    return "풀";
                case AttributeType.Light:
                    return "빛";
                case AttributeType.Dark:
                    return "어둠";
                default:
                    return "무";
            }
        }
    }
}
