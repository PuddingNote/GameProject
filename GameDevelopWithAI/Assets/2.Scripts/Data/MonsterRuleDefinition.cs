using UnityEngine;

namespace SlimeExperiment.Data
{
    [CreateAssetMenu(menuName = "SlimeExperiment/Monster Rule Definition", fileName = "MonsterRuleDefinition")]
    public sealed class MonsterRuleDefinition : ScriptableObject
    {
        [SerializeField] private int stageIndex;
        [SerializeField] private bool isBoss;
        [SerializeField] private int health;
        [SerializeField] private int minAttack;
        [SerializeField] private int maxAttack;
        [SerializeField] private float extraAttackChance;
        [SerializeField] private float dodgeChance;
        [SerializeField] private float damageReductionChance;
        [SerializeField] private float executeChance;

        public int StageIndex => stageIndex;
        public bool IsBoss => isBoss;
        public int Health => health;
        public int MinAttack => minAttack;
        public int MaxAttack => maxAttack;
        public float ExtraAttackChance => extraAttackChance;
        public float DodgeChance => dodgeChance;
        public float DamageReductionChance => damageReductionChance;
        public float ExecuteChance => executeChance;

        public static MonsterRuleDefinition Create(
            int stageValue,
            bool bossValue,
            int healthValue,
            int minAttackValue,
            int maxAttackValue,
            float extraAttackValue,
            float dodgeValue,
            float reductionValue,
            float executeValue)
        {
            MonsterRuleDefinition definition = CreateInstance<MonsterRuleDefinition>();
            definition.stageIndex = stageValue;
            definition.isBoss = bossValue;
            definition.health = healthValue;
            definition.minAttack = minAttackValue;
            definition.maxAttack = maxAttackValue;
            definition.extraAttackChance = extraAttackValue;
            definition.dodgeChance = dodgeValue;
            definition.damageReductionChance = reductionValue;
            definition.executeChance = executeValue;
            return definition;
        }
    }
}
