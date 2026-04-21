using System.Collections.Generic;

namespace SlimeExperiment.Data
{
    public sealed class PrototypeContentLibrary
    {
        private readonly List<CardDefinition> attackCards;
        private readonly List<CardDefinition> utilityCards;
        private readonly List<CardDefinition> attributeCards;
        private readonly List<RelicDefinition> relics;
        private readonly List<MonsterRuleDefinition> monsterRules;

        public PrototypeContentLibrary()
        {
            attackCards = BuildAttackCards();
            utilityCards = BuildUtilityCards();
            attributeCards = BuildAttributeCards();
            relics = BuildRelics();
            monsterRules = BuildMonsterRules();
        }

        public IReadOnlyList<CardDefinition> AttackCards => attackCards;
        public IReadOnlyList<CardDefinition> UtilityCards => utilityCards;
        public IReadOnlyList<CardDefinition> AttributeCards => attributeCards;
        public IReadOnlyList<RelicDefinition> Relics => relics;
        public IReadOnlyList<MonsterRuleDefinition> MonsterRules => monsterRules;

        private static List<CardDefinition> BuildAttackCards()
        {
            return new List<CardDefinition>
            {
                CardDefinition.CreateAttack("attack_0_10", "불안정 타격", "0~10 랜덤 공격", 0, 10),
                CardDefinition.CreateAttack("attack_1_9", "흔들리는 주먹", "1~9 랜덤 공격", 1, 9),
                CardDefinition.CreateAttack("attack_2_8", "균형 잡힌 탄성", "2~8 랜덤 공격", 2, 8),
                CardDefinition.CreateAttack("attack_3_7", "정밀 압축", "3~7 랜덤 공격", 3, 7),
                CardDefinition.CreateAttack("attack_4_6", "안정된 충격", "4~6 랜덤 공격", 4, 6),
                CardDefinition.CreateAttack("attack_fixed_5", "고정 실험식", "고정 5 공격", 5, 5)
            };
        }

        private static List<CardDefinition> BuildUtilityCards()
        {
            return new List<CardDefinition>
            {
                CardDefinition.CreateUtility("util_extra_attack", "탄성 연격", "25% 확률 추가 공격", UtilityEffectType.BonusAttackChance, 25f),
                CardDefinition.CreateUtility("util_low_roll_combo", "저위력 보정", "공격력 4 이하일 경우 50% 확률 추가 공격", UtilityEffectType.BonusAttackWhenLowRoll, 50f),
                CardDefinition.CreateUtility("util_delayed_kill", "실험 지연 독성", "15% 확률 공격 실패 -> 다음 턴 적 사망", UtilityEffectType.DelayedKillOnMiss, 15f),
                CardDefinition.CreateUtility("util_dodge", "미끄러운 회피", "25% 확률 회피", UtilityEffectType.DodgeChance, 25f),
                CardDefinition.CreateUtility("util_damage_reduce", "점액 방어막", "75% 확률 피해 50% 감소", UtilityEffectType.DamageReductionChance, 75f),
                CardDefinition.CreateUtility("util_low_health_dodge", "위기 반응", "체력 10 이하 시 40% 회피", UtilityEffectType.DodgeWhenLowHealth, 40f)
            };
        }

        private static List<CardDefinition> BuildAttributeCards()
        {
            return new List<CardDefinition>
            {
                CardDefinition.CreateAttribute("attr_neutral", "무", "기본 속성", AttributeType.Neutral),
                CardDefinition.CreateAttribute("attr_fire", "불", "풀에 강하고 물에 약함", AttributeType.Fire),
                CardDefinition.CreateAttribute("attr_water", "물", "불에 강하고 풀에 약함", AttributeType.Water),
                CardDefinition.CreateAttribute("attr_grass", "풀", "물에 강하고 불에 약함", AttributeType.Grass),
                CardDefinition.CreateAttribute("attr_light", "빛", "어둠에 강함", AttributeType.Light),
                CardDefinition.CreateAttribute("attr_dark", "어둠", "빛에 강함", AttributeType.Dark)
            };
        }

        private static List<MonsterRuleDefinition> BuildMonsterRules()
        {
            return new List<MonsterRuleDefinition>
            {
                MonsterRuleDefinition.Create(1, false, 6, 1, 3, 20f, 20f, 30f, 5f),
                MonsterRuleDefinition.Create(2, false, 12, 2, 5, 20f, 20f, 30f, 5f),
                MonsterRuleDefinition.Create(3, false, 18, 3, 6, 20f, 20f, 30f, 5f),
                MonsterRuleDefinition.Create(4, false, 24, 4, 8, 20f, 20f, 30f, 5f),
                MonsterRuleDefinition.Create(5, false, 30, 5, 10, 20f, 20f, 30f, 5f),
                MonsterRuleDefinition.Create(6, true, 40, 6, 12, 30f, 30f, 40f, 15f)
            };
        }

        private static List<RelicDefinition> BuildRelics()
        {
            return new List<RelicDefinition>
            {
                RelicDefinition.Create("relic_cotton_fist", "솜사탕 주먹", "공격력 +2", RelicCategory.BasicStat),
                RelicDefinition.Create("relic_pencil", "뾰족한 연필", "공격력 +4, 최대 체력 -2", RelicCategory.BasicStat),
                RelicDefinition.Create("relic_bat", "튼튼한 나무방망이", "공격값 최소 3 보장", RelicCategory.BasicStat),
                RelicDefinition.Create("relic_slime_mucus", "슬라임 점액", "최대 체력 +5", RelicCategory.BasicStat),
                RelicDefinition.Create("relic_bear", "거대 곰인형", "최대 체력 +10, 공격력 -2", RelicCategory.BasicStat),
                RelicDefinition.Create("relic_jelly", "달콤한 젤리", "전투 시작 시 현재 체력 +3 회복", RelicCategory.BasicStat),

                RelicDefinition.Create("relic_clockwork", "태엽 장치", "추가 공격 확률 +15%", RelicCategory.ChanceBoost, ProbabilityTag.ExtraAttack),
                RelicDefinition.Create("relic_pinwheel", "바람개비 모자", "회피율 +15%", RelicCategory.ChanceBoost, ProbabilityTag.Dodge),
                RelicDefinition.Create("relic_soft_jelly", "말랑한 젤리", "피해 감소 확률 +25%", RelicCategory.ChanceBoost, ProbabilityTag.DamageReduction),
                RelicDefinition.Create("relic_stardust", "반짝이는 별가루", "보유 중인 확률 효과 전체 +10%", RelicCategory.ChanceBoost),
                RelicDefinition.Create("relic_bomb", "째깍거리는 시한폭탄", "공격 실패 확률 +15% 증가, 공격 실패 시 다음 턴 적 사망", RelicCategory.ChanceBoost, ProbabilityTag.AttackFailure),

                RelicDefinition.Create("relic_broken_toy", "부서진 장난감", "체력 10 이하 시 공격력 +5", RelicCategory.Conditional),
                RelicDefinition.Create("relic_coward_cloak", "겁쟁이 망토", "체력 10 이하 시 회피율 +20%", RelicCategory.Conditional),
                RelicDefinition.Create("relic_surprise_box", "깜짝 상자", "첫 턴 공격력 2배", RelicCategory.Conditional),
                RelicDefinition.Create("relic_combo_balloon", "콤보 풍선", "연속 공격 성공 시 공격력 +2", RelicCategory.Conditional),
                RelicDefinition.Create("relic_ogre_mask", "화난 도깨비 가면", "피해를 입으면 다음 공격력 +3", RelicCategory.Conditional),
                RelicDefinition.Create("relic_sticky_sword", "끈적끈적한 검", "적 체력 50% 이상일 때 공격력 +4", RelicCategory.Conditional),
                RelicDefinition.Create("relic_badge", "대장님 배지", "보스 스테이지에서 공격력 +6", RelicCategory.Conditional),
                RelicDefinition.Create("relic_bouncy_ball", "탱탱볼", "피해 감소 성공 시 다음 공격력 +3", RelicCategory.Conditional),
                RelicDefinition.Create("relic_bubble", "비눗방울", "회피 성공 시 체력 +2 회복", RelicCategory.Conditional),
                RelicDefinition.Create("relic_piggy_bank", "저주받은 저금통", "매 스테이지 시작 시 최대 및 현재체력 -2, 공격력 +5 누적", RelicCategory.Conditional),

                RelicDefinition.Create("relic_rainbow", "무지개 사탕", "상성 유리 시 추가 피해 +25%", RelicCategory.AttributeSynergy),
                RelicDefinition.Create("relic_clear_cloak", "투명 망토", "상성 불리 효과 무시", RelicCategory.AttributeSynergy),
                RelicDefinition.Create("relic_gray_mucus", "회색 슬라임 점액", "무속성일 경우 공격력 +5", RelicCategory.AttributeSynergy),

                RelicDefinition.Create("relic_cursed_hammer", "저주받은 망치", "최대 체력 3으로 고정, 공격력 +15", RelicCategory.Special),
                RelicDefinition.Create("relic_melted_icecream", "녹아내린 아이스크림", "체력 회복 불가, 공격력 +5", RelicCategory.Special),
                RelicDefinition.Create("relic_skateboard", "이상한 스케이트보드", "공격값 항상 최대값, 매 턴 체력 -2", RelicCategory.Special, blockedRelicId: "relic_twin_cherry"),
                RelicDefinition.Create("relic_twin_cherry", "쌍둥이 체리", "공격값 항상 최소값, 항상 2회 공격", RelicCategory.Special, ProbabilityTag.ExtraAttack, "relic_skateboard"),
                RelicDefinition.Create("relic_even_dice", "짝수 주사위", "짝수 공격은 3배, 홀수 공격은 0", RelicCategory.Special, blockedRelicId: "relic_odd_dice"),
                RelicDefinition.Create("relic_odd_dice", "홀수 주사위", "홀수 공격은 3배, 짝수 공격은 0", RelicCategory.Special, blockedRelicId: "relic_even_dice"),
                RelicDefinition.Create("relic_strange_cloak", "이상한 망토", "첫 턴 100% 회피", RelicCategory.Special, ProbabilityTag.Dodge),
                RelicDefinition.Create("relic_jelly_bomb", "젤리 폭탄", "공격 시 20% 확률로 대상 즉사", RelicCategory.Special, ProbabilityTag.Execute),
                RelicDefinition.Create("relic_cursed_shield", "저주받은 슬라임 방패", "피격 시 20% 확률로 즉사, 회피율 +30%", RelicCategory.Special, ProbabilityTag.Dodge | ProbabilityTag.ReflectExecute),
                RelicDefinition.Create("relic_mirror", "신비한 거울", "모든 확률 2배", RelicCategory.Special)
            };
        }
    }
}
