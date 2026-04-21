using SlimeExperiment.Core;
using SlimeExperiment.Data;

namespace SlimeExperiment.Battle
{
    public static class BattleResolver
    {
        public static bool RollChance(IRandomSource randomSource, float chancePercent)
        {
            if (chancePercent <= 0f)
            {
                return false;
            }

            if (chancePercent >= 100f)
            {
                return true;
            }

            return randomSource.NextPercent() < chancePercent;
        }

        public static int RollAttack(BattleUnitState unitState, IRandomSource randomSource)
        {
            int attackValue;

            if (unitState.AlwaysMaxAttack)
            {
                attackValue = unitState.MaxAttack;
            }
            else if (unitState.AlwaysMinAttack)
            {
                attackValue = unitState.MinAttack;
            }
            else
            {
                attackValue = randomSource.NextInclusive(unitState.MinAttack, unitState.MaxAttack);
            }

            if (attackValue < unitState.GuaranteedMinimumAttack)
            {
                attackValue = unitState.GuaranteedMinimumAttack;
            }

            return attackValue;
        }

        public static float GetAttributeMultiplier(BattleUnitState attacker, BattleUnitState defender)
        {
            if (IsAdvantage(attacker.AttributeType, defender.AttributeType))
            {
                if (attacker.BonusAdvantageDamage)
                {
                    return 1.5f;
                }

                return 1.25f;
            }

            if (IsAdvantage(defender.AttributeType, attacker.AttributeType))
            {
                if (attacker.IgnoreDisadvantage)
                {
                    return 1f;
                }

                return 0.75f;
            }

            return 1f;
        }

        public static bool IsAdvantage(AttributeType attacker, AttributeType defender)
        {
            if (attacker == AttributeType.Fire && defender == AttributeType.Grass)
            {
                return true;
            }

            if (attacker == AttributeType.Water && defender == AttributeType.Fire)
            {
                return true;
            }

            if (attacker == AttributeType.Grass && defender == AttributeType.Water)
            {
                return true;
            }

            if (attacker == AttributeType.Light && defender == AttributeType.Dark)
            {
                return true;
            }

            if (attacker == AttributeType.Dark && defender == AttributeType.Light)
            {
                return true;
            }

            return false;
        }

        public static float ClampProbability(float chancePercent)
        {
            if (chancePercent < 0f)
            {
                return 0f;
            }

            if (chancePercent > 100f)
            {
                return 100f;
            }

            return chancePercent;
        }
    }
}
