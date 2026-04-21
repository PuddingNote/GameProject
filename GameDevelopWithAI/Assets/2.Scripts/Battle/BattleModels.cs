using System;
using System.Collections.Generic;
using SlimeExperiment.Data;

namespace SlimeExperiment.Battle
{
    public enum BattleEventType
    {
        Info,
        TurnStart,
        Damage,
        Dodge,
        Reduction,
        Defeat
    }

    [Serializable]
    public sealed class ExperimentBuildResult
    {
        public CardDefinition AttackCard;
        public CardDefinition UtilityCard;
        public CardDefinition AttributeCard;
        public int BaseHealth = 20;
        public int MinAttack;
        public int MaxAttack;
        public AttributeType AttributeType;
        public UtilityEffectType UtilityEffectType;
        public float UtilityChancePercent;

        public string ToSummaryText()
        {
            return $"{AttackCard.DisplayName}\n{UtilityCard.DisplayName}\n{AttributeCard.DisplayName}";
        }
    }

    [Serializable]
    public sealed class BattleUnitState
    {
        public string DisplayName;
        public bool IsPlayer;
        public bool IsBoss;
        public AttributeType AttributeType;
        public int MaxHealth;
        public int CurrentHealth;
        public int MinAttack;
        public int MaxAttack;
        public float ExtraAttackChance;
        public float LowRollExtraAttackChance;
        public float AttackFailureChance;
        public float DodgeChance;
        public float LowHealthDodgeChance;
        public float DamageReductionChance;
        public float ExecuteChance;
        public float ReflectExecuteChance;
        public float DamageReductionMultiplier = 0.5f;
        public bool AttackFailureCausesDelayedKill;
        public bool AlwaysMaxAttack;
        public bool AlwaysMinAttack;
        public bool AlwaysTwoAttacks;
        public bool EvenDiceMode;
        public bool OddDiceMode;
        public bool FirstTurnGuaranteedDodge;
        public bool NoHealing;
        public bool IgnoreDisadvantage;
        public bool BonusAdvantageDamage;
        public bool LoseHealthEachTurn;
        public int TurnHealthLoss;
        public int GuaranteedMinimumAttack;
        public int FlatAttackBonus;
        public int BonusAttackWhenLowHealth;
        public int BonusDodgeWhenLowHealth;
        public int FirstTurnDamageMultiplier = 1;
        public int BossAttackBonus;
        public int AttackBonusWhenTargetHealthy;
        public int AttackBonusAfterTakingDamage;
        public int AttackBonusAfterReduction;
        public int NeutralAttackBonus;
        public int HealOnBattleStart;
        public int HealOnDodge;
        public int NextAttackFlatBonus;
        public int SuccessfulHitsInRow;
        public bool HasTakenDamageSinceLastAction;
        public bool FirstActionCompleted;
        public bool FirstTurnDodgeConsumed;
        public bool PendingDelayedDeath;
        public List<string> RelicIds = new List<string>();

        public bool IsAlive => CurrentHealth > 0;
        public bool IsLowHealth => CurrentHealth <= 10;

        public BattleUnitState Clone()
        {
            BattleUnitState clone = (BattleUnitState)MemberwiseClone();
            clone.RelicIds = new List<string>(RelicIds);
            return clone;
        }
    }

    [Serializable]
    public sealed class BattleTurnEvent
    {
        public BattleEventType EventType;
        public string Message;
        public bool AttackerIsPlayer;
        public bool DefenderIsPlayer;
        public int Damage;
        public int PlayerHealthAfter;
        public int MonsterHealthAfter;
    }

    [Serializable]
    public sealed class BattleReport
    {
        public BattleOutcome Outcome;
        public int StageIndex;
        public bool IsBossStage;
        public BattleUnitState InitialPlayerState;
        public BattleUnitState InitialMonsterState;
        public BattleUnitState FinalPlayerState;
        public BattleUnitState FinalMonsterState;
        public List<BattleTurnEvent> Events = new List<BattleTurnEvent>();
    }

    [Serializable]
    public sealed class MonsterEncounter
    {
        public MonsterRuleDefinition Rule;
        public AttributeType AttributeType;
        public MonsterUtilityType UtilityType;
        public string DisplayName;
        public string RouletteSummary;
    }
}
