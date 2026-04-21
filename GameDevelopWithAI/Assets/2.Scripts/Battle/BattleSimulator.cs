using SlimeExperiment.Core;
using SlimeExperiment.Data;

namespace SlimeExperiment.Battle
{
    public sealed class BattleSimulator
    {
        private readonly IRandomSource randomSource;

        public BattleSimulator(IRandomSource randomSource)
        {
            this.randomSource = randomSource;
        }

        public BattleReport Simulate(BattleUnitState playerState, BattleUnitState monsterState, int stageIndex, bool isBossStage)
        {
            BattleReport report = new BattleReport
            {
                StageIndex = stageIndex,
                IsBossStage = isBossStage,
                InitialPlayerState = playerState.Clone(),
                InitialMonsterState = monsterState.Clone(),
                FinalPlayerState = playerState,
                FinalMonsterState = monsterState
            };

            int turn = 1;

            while (playerState.IsAlive && monsterState.IsAlive && turn <= 30)
            {
                AddEvent(report, BattleEventType.TurnStart, $"턴 {turn} 시작");

                if (ResolvePendingDeath(playerState, report, turn) || ResolvePendingDeath(monsterState, report, turn))
                {
                    if (playerState.IsAlive == false || monsterState.IsAlive == false)
                    {
                        break;
                    }
                }

                ExecuteAction(playerState, monsterState, report, turn);
                if (monsterState.IsAlive == false)
                {
                    break;
                }

                ExecuteAction(monsterState, playerState, report, turn);
                if (playerState.IsAlive == false)
                {
                    break;
                }

                ApplyEndOfRoundEffects(playerState, report);
                ApplyEndOfRoundEffects(monsterState, report);

                turn++;
            }

            report.Outcome = playerState.IsAlive ? BattleOutcome.Victory : BattleOutcome.Defeat;
            return report;
        }

        private bool ResolvePendingDeath(BattleUnitState unitState, BattleReport report, int turn)
        {
            if (unitState.PendingDelayedDeath == false || unitState.IsAlive == false)
            {
                return false;
            }

            unitState.PendingDelayedDeath = false;
            unitState.CurrentHealth = 0;
            AddEvent(report, BattleEventType.Defeat, $"{unitState.DisplayName}이(가) 지연 폭발로 쓰러졌습니다.", defender: unitState);
            return true;
        }

        private void ExecuteAction(BattleUnitState attacker, BattleUnitState defender, BattleReport report, int turn)
        {
            if (attacker.IsAlive == false || defender.IsAlive == false)
            {
                return;
            }

            int attackCount = 1;
            if (attacker.AlwaysTwoAttacks)
            {
                attackCount++;
            }

            if (BattleResolver.RollChance(randomSource, attacker.ExtraAttackChance))
            {
                attackCount++;
                AddEvent(report, BattleEventType.Info, $"{attacker.DisplayName}이(가) 추가 공격 기회를 얻었습니다.", attacker);
            }

            int attackIndex = 0;
            while (attackIndex < attackCount && attacker.IsAlive && defender.IsAlive)
            {
                ResolveSingleAttack(attacker, defender, report, turn, ref attackCount);
                attackIndex++;
            }

            attacker.HasTakenDamageSinceLastAction = false;
            attacker.FirstActionCompleted = true;
        }

        private void ResolveSingleAttack(
            BattleUnitState attacker,
            BattleUnitState defender,
            BattleReport report,
            int turn,
            ref int attackCount)
        {
            if (BattleResolver.RollChance(randomSource, attacker.AttackFailureChance))
            {
                string message = $"{attacker.DisplayName}의 공격이 빗나갔습니다.";
                if (attacker.AttackFailureCausesDelayedKill)
                {
                    defender.PendingDelayedDeath = true;
                    message += $" {defender.DisplayName}은(는) 다음 턴 시작 시 사망합니다.";
                }

                attacker.SuccessfulHitsInRow = 0;
                AddEvent(report, BattleEventType.Info, message, attacker, defender);
                return;
            }

            int rolledAttack = BattleResolver.RollAttack(attacker, randomSource);
            if (rolledAttack <= 4 && BattleResolver.RollChance(randomSource, attacker.LowRollExtraAttackChance))
            {
                attackCount++;
                AddEvent(report, BattleEventType.Info, $"{attacker.DisplayName}이(가) 저위력 보정으로 한 번 더 공격합니다.", attacker);
            }

            int damage = rolledAttack;
            damage += attacker.NextAttackFlatBonus;
            attacker.NextAttackFlatBonus = 0;

            if (attacker.IsLowHealth)
            {
                damage += attacker.BonusAttackWhenLowHealth;
            }

            if (defender.CurrentHealth * 2 >= defender.MaxHealth)
            {
                damage += attacker.AttackBonusWhenTargetHealthy;
            }

            if (attacker.HasTakenDamageSinceLastAction)
            {
                damage += attacker.AttackBonusAfterTakingDamage;
            }

            if (attacker.SuccessfulHitsInRow > 0 && attacker.RelicIds.Contains("relic_combo_balloon"))
            {
                damage += 2;
            }

            damage += attacker.BossAttackBonus;

            if (attacker.AttributeType == AttributeType.Neutral)
            {
                damage += attacker.NeutralAttackBonus;
            }

            if (attacker.FirstActionCompleted == false && attacker.FirstTurnDamageMultiplier > 1)
            {
                damage *= attacker.FirstTurnDamageMultiplier;
            }

            if (attacker.EvenDiceMode)
            {
                damage = damage % 2 == 0 ? damage * 3 : 0;
            }

            if (attacker.OddDiceMode)
            {
                damage = damage % 2 != 0 ? damage * 3 : 0;
            }

            damage = (int)System.MathF.Round(damage * BattleResolver.GetAttributeMultiplier(attacker, defender));
            if (damage < 0)
            {
                damage = 0;
            }

            if (ResolveDodge(attacker, defender, report))
            {
                attacker.SuccessfulHitsInRow = 0;
                return;
            }

            bool reductionTriggered = ResolveDamageReduction(defender, report);
            if (reductionTriggered)
            {
                damage = (int)System.MathF.Round(damage * defender.DamageReductionMultiplier);
                if (damage < 0)
                {
                    damage = 0;
                }
            }

            if (BattleResolver.RollChance(randomSource, attacker.ExecuteChance))
            {
                damage = defender.CurrentHealth;
                AddEvent(report, BattleEventType.Info, $"{attacker.DisplayName}의 즉사 효과가 발동했습니다.", attacker, defender);
            }

            defender.CurrentHealth -= damage;
            if (defender.CurrentHealth < 0)
            {
                defender.CurrentHealth = 0;
            }

            AddEvent(
                report,
                BattleEventType.Damage,
                $"{attacker.DisplayName}이(가) {defender.DisplayName}에게 {damage} 피해를 입혔습니다. ({defender.CurrentHealth}/{defender.MaxHealth})",
                attacker,
                defender,
                damage);

            if (damage > 0)
            {
                attacker.SuccessfulHitsInRow++;
                defender.HasTakenDamageSinceLastAction = true;
            }
            else
            {
                attacker.SuccessfulHitsInRow = 0;
            }

            if (BattleResolver.RollChance(randomSource, defender.ReflectExecuteChance) && attacker.IsAlive)
            {
                attacker.CurrentHealth = 0;
                AddEvent(report, BattleEventType.Defeat, $"{defender.DisplayName}의 저주받은 방패가 반격해 {attacker.DisplayName}이(가) 즉사했습니다.", defender, attacker);
            }
        }

        private bool ResolveDodge(BattleUnitState attacker, BattleUnitState defender, BattleReport report)
        {
            float dodgeChance = defender.DodgeChance;
            if (defender.IsLowHealth)
            {
                dodgeChance += defender.LowHealthDodgeChance;
            }

            dodgeChance = BattleResolver.ClampProbability(dodgeChance);

            bool dodged = false;
            if (defender.FirstTurnGuaranteedDodge && defender.FirstTurnDodgeConsumed == false)
            {
                dodged = true;
                defender.FirstTurnDodgeConsumed = true;
            }
            else if (BattleResolver.RollChance(randomSource, dodgeChance))
            {
                dodged = true;
            }

            if (dodged == false)
            {
                return false;
            }

            if (defender.HealOnDodge > 0 && defender.NoHealing == false)
            {
                defender.CurrentHealth += defender.HealOnDodge;
                if (defender.CurrentHealth > defender.MaxHealth)
                {
                    defender.CurrentHealth = defender.MaxHealth;
                }
            }

            AddEvent(report, BattleEventType.Dodge, $"{defender.DisplayName}이(가) 공격을 회피했습니다.", attacker, defender);
            return true;
        }

        private bool ResolveDamageReduction(BattleUnitState defender, BattleReport report)
        {
            if (BattleResolver.RollChance(randomSource, defender.DamageReductionChance) == false)
            {
                return false;
            }

            defender.NextAttackFlatBonus += defender.AttackBonusAfterReduction;
            AddEvent(report, BattleEventType.Reduction, $"{defender.DisplayName}이(가) 피해를 줄였습니다.", defender);
            return true;
        }

        private void ApplyEndOfRoundEffects(BattleUnitState unitState, BattleReport report)
        {
            if (unitState.IsAlive == false)
            {
                return;
            }

            if (unitState.LoseHealthEachTurn)
            {
                unitState.CurrentHealth -= unitState.TurnHealthLoss;
                if (unitState.CurrentHealth < 0)
                {
                    unitState.CurrentHealth = 0;
                }

                AddEvent(report, BattleEventType.Info, $"{unitState.DisplayName}이(가) 지속 대가로 {unitState.TurnHealthLoss} 체력을 잃었습니다.", defender: unitState);
            }
        }

        private static void AddEvent(
            BattleReport report,
            BattleEventType eventType,
            string message,
            BattleUnitState attacker = null,
            BattleUnitState defender = null,
            int damage = 0)
        {
            report.Events.Add(new BattleTurnEvent
            {
                EventType = eventType,
                Message = message,
                AttackerIsPlayer = attacker != null && attacker.IsPlayer,
                DefenderIsPlayer = defender != null && defender.IsPlayer,
                Damage = damage,
                PlayerHealthAfter = report.FinalPlayerState.CurrentHealth,
                MonsterHealthAfter = report.FinalMonsterState.CurrentHealth
            });
        }
    }
}
