using UnityEngine;

namespace SlimeExperiment.Data
{
    [CreateAssetMenu(menuName = "SlimeExperiment/Card Definition", fileName = "CardDefinition")]
    public sealed class CardDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private CardCategory category;
        [SerializeField] private int minAttack;
        [SerializeField] private int maxAttack;
        [SerializeField] private UtilityEffectType utilityEffectType;
        [SerializeField] private float utilityChancePercent;
        [SerializeField] private AttributeType attributeType;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public CardCategory Category => category;
        public int MinAttack => minAttack;
        public int MaxAttack => maxAttack;
        public UtilityEffectType UtilityEffectType => utilityEffectType;
        public float UtilityChancePercent => utilityChancePercent;
        public AttributeType AttributeType => attributeType;

        public static CardDefinition CreateAttack(string idValue, string title, string text, int minValue, int maxValue)
        {
            CardDefinition definition = CreateInstance<CardDefinition>();
            definition.id = idValue;
            definition.displayName = title;
            definition.description = text;
            definition.category = CardCategory.Attack;
            definition.minAttack = minValue;
            definition.maxAttack = maxValue;
            definition.attributeType = AttributeType.Neutral;
            return definition;
        }

        public static CardDefinition CreateUtility(string idValue, string title, string text, UtilityEffectType effectType, float chancePercent)
        {
            CardDefinition definition = CreateInstance<CardDefinition>();
            definition.id = idValue;
            definition.displayName = title;
            definition.description = text;
            definition.category = CardCategory.Utility;
            definition.utilityEffectType = effectType;
            definition.utilityChancePercent = chancePercent;
            definition.attributeType = AttributeType.Neutral;
            return definition;
        }

        public static CardDefinition CreateAttribute(string idValue, string title, string text, AttributeType attribute)
        {
            CardDefinition definition = CreateInstance<CardDefinition>();
            definition.id = idValue;
            definition.displayName = title;
            definition.description = text;
            definition.category = CardCategory.Attribute;
            definition.attributeType = attribute;
            return definition;
        }
    }
}
