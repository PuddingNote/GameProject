using System;
using System.Collections.Generic;
using SlimeExperiment.Data;
using UnityEngine;

namespace SlimeExperiment.UI
{
    [CreateAssetMenu(menuName = "SlimeExperiment/Prototype Sprite Catalog", fileName = "PrototypeSpriteCatalog")]
    public sealed class PrototypeSpriteCatalog : ScriptableObject
    {
        [SerializeField] private Sprite cardFrameSprite;
        [SerializeField] private Sprite relicFrameSprite;
        [SerializeField] private Sprite infoFrameSprite;
        [SerializeField] private List<AttributeSpriteEntry> characterSprites = new List<AttributeSpriteEntry>();
        [SerializeField] private List<AttributeSpriteEntry> characterHandSprites = new List<AttributeSpriteEntry>();
        [SerializeField] private List<AttributeSpriteEntry> monsterSprites = new List<AttributeSpriteEntry>();
        [SerializeField] private List<RelicSpriteEntry> relicSprites = new List<RelicSpriteEntry>();

        public Sprite CardFrameSprite => cardFrameSprite;
        public Sprite RelicFrameSprite => relicFrameSprite;
        public Sprite InfoFrameSprite => infoFrameSprite;

        public Sprite GetCharacterSprite(AttributeType attributeType)
        {
            return GetAttributeSprite(characterSprites, attributeType);
        }

        public Sprite GetMonsterSprite(AttributeType attributeType)
        {
            return GetAttributeSprite(monsterSprites, attributeType);
        }

        public Sprite GetCharacterHandSprite(AttributeType attributeType)
        {
            return GetAttributeSprite(characterHandSprites, attributeType);
        }

        public Sprite GetRelicSprite(string relicId)
        {
            for (int index = 0; index < relicSprites.Count; index++)
            {
                if (relicSprites[index].RelicId == relicId)
                {
                    return relicSprites[index].Sprite;
                }
            }

            return null;
        }

        private static Sprite GetAttributeSprite(List<AttributeSpriteEntry> entries, AttributeType attributeType)
        {
            for (int index = 0; index < entries.Count; index++)
            {
                if (entries[index].AttributeType == attributeType)
                {
                    return entries[index].Sprite;
                }
            }

            return null;
        }

        [Serializable]
        public sealed class AttributeSpriteEntry
        {
            public AttributeType AttributeType;
            public Sprite Sprite;
        }

        [Serializable]
        public sealed class RelicSpriteEntry
        {
            public string RelicId;
            public Sprite Sprite;
        }
    }
}
