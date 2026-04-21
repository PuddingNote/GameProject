using System;

namespace SlimeExperiment.Data
{
    public enum CardCategory
    {
        Attack,
        Utility,
        Attribute
    }

    public enum UtilityEffectType
    {
        None,
        BonusAttackChance,
        BonusAttackWhenLowRoll,
        DelayedKillOnMiss,
        DodgeChance,
        DamageReductionChance,
        DodgeWhenLowHealth
    }

    public enum AttributeType
    {
        Neutral,
        Fire,
        Water,
        Grass,
        Light,
        Dark
    }

    public enum RelicCategory
    {
        BasicStat,
        ChanceBoost,
        Conditional,
        AttributeSynergy,
        Special
    }

    public enum MonsterUtilityType
    {
        None,
        ExtraAttack,
        Dodge,
        DamageReduction,
        Execute
    }

    public enum DraftPhase
    {
        Attack,
        Utility,
        Attribute,
        Complete
    }

    public enum ScreenState
    {
        MainMenu,
        Logbook,
        Codex,
        Draft,
        Roulette,
        Battle,
        Reward,
        Result
    }

    public enum BattleOutcome
    {
        Victory,
        Defeat
    }

    [Flags]
    public enum ProbabilityTag
    {
        None = 0,
        ExtraAttack = 1 << 0,
        Dodge = 1 << 1,
        DamageReduction = 1 << 2,
        AttackFailure = 1 << 3,
        Execute = 1 << 4,
        ReflectExecute = 1 << 5
    }
}
