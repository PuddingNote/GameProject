using UnityEngine;

namespace SlimeExperiment.Data
{
    [CreateAssetMenu(menuName = "SlimeExperiment/Relic Definition", fileName = "RelicDefinition")]
    public sealed class RelicDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private RelicCategory category;
        [SerializeField] private ProbabilityTag probabilityTags;
        [SerializeField] private string blocksRelicId;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public RelicCategory Category => category;
        public ProbabilityTag ProbabilityTags => probabilityTags;
        public string BlocksRelicId => blocksRelicId;

        public static RelicDefinition Create(
            string idValue,
            string title,
            string text,
            RelicCategory relicCategory,
            ProbabilityTag tags = ProbabilityTag.None,
            string blockedRelicId = "")
        {
            RelicDefinition definition = CreateInstance<RelicDefinition>();
            definition.id = idValue;
            definition.displayName = title;
            definition.description = text;
            definition.category = relicCategory;
            definition.probabilityTags = tags;
            definition.blocksRelicId = blockedRelicId;
            return definition;
        }
    }
}
