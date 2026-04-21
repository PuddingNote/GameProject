using System.Collections.Generic;
using SlimeExperiment.Data;

namespace SlimeExperiment.Battle
{
    public static class RelicEffectResolver
    {
        public static BattleUnitState BuildPlayerState(
            ExperimentBuildResult buildResult,
            IReadOnlyList<RelicDefinition> relics,
            int currentHealth,
            int piggyBankStacks,
            bool isBossStage)
        {
            BattleUnitState state = new BattleUnitState
            {
                DisplayName = "실험체 슬라임",
                IsPlayer = true,
                IsBoss = false,
                AttributeType = buildResult.AttributeType,
                MaxHealth = buildResult.BaseHealth,
                CurrentHealth = currentHealth,
                MinAttack = buildResult.MinAttack,
                MaxAttack = buildResult.MaxAttack,
                DamageReductionMultiplier = 0.5f
            };

            ApplyCardUtility(buildResult, state);

            for (int index = 0; index < relics.Count; index++)
            {
                RelicDefinition relic = relics[index];
                state.RelicIds.Add(relic.Id);
                ApplyRelic(relic.Id, state, isBossStage);
            }

            ApplyPiggyBankStacks(state, piggyBankStacks);
            ApplyProbabilityBoosts(relics, state);

            if (state.AttributeType == AttributeType.Neutral && HasRelic(relics, "relic_gray_mucus"))
            {
                state.FlatAttackBonus += 5;
            }

            state.MinAttack += state.FlatAttackBonus;
            state.MaxAttack += state.FlatAttackBonus;

            if (state.MinAttack < 0)
            {
                state.MinAttack = 0;
            }

            if (state.MaxAttack < state.MinAttack)
            {
                state.MaxAttack = state.MinAttack;
            }

            if (state.MaxHealth < 1)
            {
                state.MaxHealth = 1;
            }

            if (state.CurrentHealth <= 0)
            {
                state.CurrentHealth = state.MaxHealth;
            }

            if (state.CurrentHealth > state.MaxHealth)
            {
                state.CurrentHealth = state.MaxHealth;
            }

            if (state.HealOnBattleStart > 0 && state.NoHealing == false)
            {
                state.CurrentHealth += state.HealOnBattleStart;
                if (state.CurrentHealth > state.MaxHealth)
                {
                    state.CurrentHealth = state.MaxHealth;
                }
            }

            return state;
        }

        private static void ApplyCardUtility(ExperimentBuildResult buildResult, BattleUnitState state)
        {
            switch (buildResult.UtilityEffectType)
            {
                case UtilityEffectType.BonusAttackChance:
                    state.ExtraAttackChance += buildResult.UtilityChancePercent;
                    break;
                case UtilityEffectType.BonusAttackWhenLowRoll:
                    state.LowRollExtraAttackChance += buildResult.UtilityChancePercent;
                    break;
                case UtilityEffectType.DelayedKillOnMiss:
                    state.AttackFailureChance += buildResult.UtilityChancePercent;
                    state.AttackFailureCausesDelayedKill = true;
                    break;
                case UtilityEffectType.DodgeChance:
                    state.DodgeChance += buildResult.UtilityChancePercent;
                    break;
                case UtilityEffectType.DamageReductionChance:
                    state.DamageReductionChance += buildResult.UtilityChancePercent;
                    break;
                case UtilityEffectType.DodgeWhenLowHealth:
                    state.LowHealthDodgeChance += buildResult.UtilityChancePercent;
                    break;
            }
        }

        private static void ApplyRelic(string relicId, BattleUnitState state, bool isBossStage)
        {
            switch (relicId)
            {
                case "relic_cotton_fist":
                    state.FlatAttackBonus += 2;
                    break;
                case "relic_pencil":
                    state.FlatAttackBonus += 4;
                    state.MaxHealth -= 2;
                    break;
                case "relic_bat":
                    state.GuaranteedMinimumAttack = 3;
                    break;
                case "relic_slime_mucus":
                    state.MaxHealth += 5;
                    break;
                case "relic_bear":
                    state.MaxHealth += 10;
                    state.FlatAttackBonus -= 2;
                    break;
                case "relic_jelly":
                    state.HealOnBattleStart += 3;
                    break;
                case "relic_clockwork":
                    state.ExtraAttackChance += 15f;
                    break;
                case "relic_pinwheel":
                    state.DodgeChance += 15f;
                    break;
                case "relic_soft_jelly":
                    state.DamageReductionChance += 25f;
                    break;
                case "relic_bomb":
                    state.AttackFailureChance += 15f;
                    state.AttackFailureCausesDelayedKill = true;
                    break;
                case "relic_broken_toy":
                    state.BonusAttackWhenLowHealth += 5;
                    break;
                case "relic_coward_cloak":
                    state.BonusDodgeWhenLowHealth += 20;
                    break;
                case "relic_surprise_box":
                    state.FirstTurnDamageMultiplier = 2;
                    break;
                case "relic_combo_balloon":
                    break;
                case "relic_ogre_mask":
                    state.AttackBonusAfterTakingDamage += 3;
                    break;
                case "relic_sticky_sword":
                    state.AttackBonusWhenTargetHealthy += 4;
                    break;
                case "relic_badge":
                    if (isBossStage)
                    {
                        state.BossAttackBonus += 6;
                    }

                    break;
                case "relic_bouncy_ball":
                    state.AttackBonusAfterReduction += 3;
                    break;
                case "relic_bubble":
                    state.HealOnDodge += 2;
                    break;
                case "relic_rainbow":
                    state.BonusAdvantageDamage = true;
                    break;
                case "relic_clear_cloak":
                    state.IgnoreDisadvantage = true;
                    break;
                case "relic_cursed_hammer":
                    state.MaxHealth = 3;
                    state.FlatAttackBonus += 15;
                    break;
                case "relic_melted_icecream":
                    state.NoHealing = true;
                    state.FlatAttackBonus += 5;
                    break;
                case "relic_skateboard":
                    state.AlwaysMaxAttack = true;
                    state.LoseHealthEachTurn = true;
                    state.TurnHealthLoss = 2;
                    break;
                case "relic_twin_cherry":
                    state.AlwaysMinAttack = true;
                    state.AlwaysTwoAttacks = true;
                    break;
                case "relic_even_dice":
                    state.EvenDiceMode = true;
                    break;
                case "relic_odd_dice":
                    state.OddDiceMode = true;
                    break;
                case "relic_strange_cloak":
                    state.FirstTurnGuaranteedDodge = true;
                    break;
                case "relic_jelly_bomb":
                    state.ExecuteChance += 20f;
                    break;
                case "relic_cursed_shield":
                    state.ReflectExecuteChance += 20f;
                    state.DodgeChance += 30f;
                    break;
            }
        }

        private static void ApplyPiggyBankStacks(BattleUnitState state, int piggyBankStacks)
        {
            if (piggyBankStacks <= 0)
            {
                return;
            }

            state.MaxHealth -= piggyBankStacks * 2;
            state.CurrentHealth -= piggyBankStacks * 2;
            state.FlatAttackBonus += piggyBankStacks * 5;
        }

        private static void ApplyProbabilityBoosts(IReadOnlyList<RelicDefinition> relics, BattleUnitState state)
        {
            bool hasStarDust = HasRelic(relics, "relic_stardust");
            bool hasMirror = HasRelic(relics, "relic_mirror");

            if (hasStarDust)
            {
                if (state.ExtraAttackChance > 0f)
                {
                    state.ExtraAttackChance += 10f;
                }

                if (state.DodgeChance > 0f)
                {
                    state.DodgeChance += 10f;
                }

                if (state.LowHealthDodgeChance > 0f)
                {
                    state.LowHealthDodgeChance += 10f;
                }

                if (state.DamageReductionChance > 0f)
                {
                    state.DamageReductionChance += 10f;
                }

                if (state.AttackFailureChance > 0f)
                {
                    state.AttackFailureChance += 10f;
                }

                if (state.ExecuteChance > 0f)
                {
                    state.ExecuteChance += 10f;
                }

                if (state.ReflectExecuteChance > 0f)
                {
                    state.ReflectExecuteChance += 10f;
                }
            }

            if (hasMirror)
            {
                state.ExtraAttackChance *= 2f;
                state.LowRollExtraAttackChance *= 2f;
                state.DodgeChance *= 2f;
                state.LowHealthDodgeChance *= 2f;
                state.DamageReductionChance *= 2f;
                state.AttackFailureChance *= 2f;
                state.ExecuteChance *= 2f;
                state.ReflectExecuteChance *= 2f;
            }

            state.ExtraAttackChance = BattleResolver.ClampProbability(state.ExtraAttackChance);
            state.LowRollExtraAttackChance = BattleResolver.ClampProbability(state.LowRollExtraAttackChance);
            state.DodgeChance = BattleResolver.ClampProbability(state.DodgeChance);
            state.LowHealthDodgeChance = BattleResolver.ClampProbability(state.LowHealthDodgeChance + state.BonusDodgeWhenLowHealth);
            state.DamageReductionChance = BattleResolver.ClampProbability(state.DamageReductionChance);
            state.AttackFailureChance = BattleResolver.ClampProbability(state.AttackFailureChance);
            state.ExecuteChance = BattleResolver.ClampProbability(state.ExecuteChance);
            state.ReflectExecuteChance = BattleResolver.ClampProbability(state.ReflectExecuteChance);
        }

        private static bool HasRelic(IReadOnlyList<RelicDefinition> relics, string relicId)
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
    }
}
