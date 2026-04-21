using System.Collections.Generic;
using SlimeExperiment.Data;
using SlimeExperiment.UI;
using UnityEditor;
using UnityEngine;

namespace SlimeExperiment.Editor
{
    public static class PrototypeSpriteCatalogBuilder
    {
        private const string CatalogAssetPath = "Assets/Resources/PrototypeSpriteCatalog.asset";

        [MenuItem("SlimeExperiment/Build Prototype Sprite Catalog")]
        public static void BuildCatalog()
        {
            EnsureFolder("Assets/Resources");

            PrototypeSpriteCatalog catalog = AssetDatabase.LoadAssetAtPath<PrototypeSpriteCatalog>(CatalogAssetPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<PrototypeSpriteCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogAssetPath);
            }

            SerializedObject serializedObject = new SerializedObject(catalog);
            AssignSprite(serializedObject, "cardFrameSprite", "Assets/5.Sprites/4. UI/1. 카드 테두리.png", "1. 카드 테두리_0");
            AssignSprite(serializedObject, "relicFrameSprite", "Assets/5.Sprites/4. UI/2. 유물 테두리.png");
            AssignSprite(serializedObject, "infoFrameSprite", "Assets/5.Sprites/4. UI/3. 텍스트 테두리.png");

            AssignAttributeSprites(serializedObject.FindProperty("characterSprites"), new Dictionary<AttributeType, string>
            {
                { AttributeType.Neutral, "Assets/5.Sprites/1. Characters/1. 무속성 캐릭터.png" },
                { AttributeType.Fire, "Assets/5.Sprites/1. Characters/2. 불속성 캐릭터.png" },
                { AttributeType.Water, "Assets/5.Sprites/1. Characters/3. 물속성 캐릭터.png" },
                { AttributeType.Grass, "Assets/5.Sprites/1. Characters/4. 풀속성 캐릭터.png" },
                { AttributeType.Light, "Assets/5.Sprites/1. Characters/5. 빛속성 캐릭터.png" },
                { AttributeType.Dark, "Assets/5.Sprites/1. Characters/6. 어둠속성 캐릭터.png" }
            });

            AssignAttributeSprites(serializedObject.FindProperty("characterHandSprites"), new Dictionary<AttributeType, string>
            {
                { AttributeType.Neutral, "Assets/5.Sprites/1. Characters/1. 무속성 캐릭터 손.png" },
                { AttributeType.Fire, "Assets/5.Sprites/1. Characters/2. 불속성 캐릭터 손.png" },
                { AttributeType.Water, "Assets/5.Sprites/1. Characters/3. 물속성 캐릭터 손.png" },
                { AttributeType.Grass, "Assets/5.Sprites/1. Characters/4. 풀속성 캐릭터 손.png" },
                { AttributeType.Light, "Assets/5.Sprites/1. Characters/5. 빛속성 캐릭터 손.png" },
                { AttributeType.Dark, "Assets/5.Sprites/1. Characters/6. 어둠속성 캐릭터 손.png" }
            });

            AssignAttributeSprites(serializedObject.FindProperty("monsterSprites"), new Dictionary<AttributeType, string>
            {
                { AttributeType.Neutral, "Assets/5.Sprites/2. Monsters/1. 무속성 몬스터.png" },
                { AttributeType.Fire, "Assets/5.Sprites/2. Monsters/2. 불속성 몬스터.png" },
                { AttributeType.Water, "Assets/5.Sprites/2. Monsters/3. 물속성 몬스터.png" },
                { AttributeType.Grass, "Assets/5.Sprites/2. Monsters/4. 풀속성 몬스터.png" },
                { AttributeType.Light, "Assets/5.Sprites/2. Monsters/5. 빛속성 몬스터.png" },
                { AttributeType.Dark, "Assets/5.Sprites/2. Monsters/6. 어둠속성 몬스터.png" }
            });

            AssignRelicSprites(serializedObject.FindProperty("relicSprites"), BuildRelicMap());
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Dictionary<string, string> BuildRelicMap()
        {
            return new Dictionary<string, string>
            {
                { "relic_cotton_fist", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/1. 솜사탕 주먹.png" },
                { "relic_pencil", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/2. 뾰족한 연필.png" },
                { "relic_bat", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/3. 튼튼한 나무방망이.png" },
                { "relic_slime_mucus", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/4. 슬라임 점액.png" },
                { "relic_bear", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/5. 거대 곰인형.png" },
                { "relic_jelly", "Assets/5.Sprites/3. Relics/1. 기본 스탯형/6. 달콤한 젤리.png" },
                { "relic_clockwork", "Assets/5.Sprites/3. Relics/2. 확률 강화형/1. 태엽 장치.png" },
                { "relic_pinwheel", "Assets/5.Sprites/3. Relics/2. 확률 강화형/2. 바람개비 모자.png" },
                { "relic_soft_jelly", "Assets/5.Sprites/3. Relics/2. 확률 강화형/3. 말랑한 젤리.png" },
                { "relic_stardust", "Assets/5.Sprites/3. Relics/2. 확률 강화형/4. 반짝이는 별가루.png" },
                { "relic_bomb", "Assets/5.Sprites/3. Relics/2. 확률 강화형/5. 째깍거리는 시한폭탄.png" },
                { "relic_broken_toy", "Assets/5.Sprites/3. Relics/3. 조건형/1. 부서진 장난감.png" },
                { "relic_coward_cloak", "Assets/5.Sprites/3. Relics/3. 조건형/2. 겁쟁이 망토.png" },
                { "relic_surprise_box", "Assets/5.Sprites/3. Relics/3. 조건형/3. 깜짝상자.png" },
                { "relic_combo_balloon", "Assets/5.Sprites/3. Relics/3. 조건형/4. 콤보 풍선.png" },
                { "relic_ogre_mask", "Assets/5.Sprites/3. Relics/3. 조건형/5. 화난 도깨비가면.png" },
                { "relic_sticky_sword", "Assets/5.Sprites/3. Relics/3. 조건형/6. 끈적끈적한 검.png" },
                { "relic_badge", "Assets/5.Sprites/3. Relics/3. 조건형/7. 대장님 배지.png" },
                { "relic_bouncy_ball", "Assets/5.Sprites/3. Relics/3. 조건형/8. 탱탱볼.png" },
                { "relic_bubble", "Assets/5.Sprites/3. Relics/3. 조건형/9. 비눗방울.png" },
                { "relic_piggy_bank", "Assets/5.Sprites/3. Relics/3. 조건형/10. 저주받은 저금통.png" },
                { "relic_rainbow", "Assets/5.Sprites/3. Relics/4. 속성 시너지형/1. 무지개 사탕.png" },
                { "relic_clear_cloak", "Assets/5.Sprites/3. Relics/4. 속성 시너지형/2. 투명망토.png" },
                { "relic_gray_mucus", "Assets/5.Sprites/3. Relics/4. 속성 시너지형/3. 회색 슬라임 점액.png" },
                { "relic_cursed_hammer", "Assets/5.Sprites/3. Relics/5. 특수형/1. 저주받은 망치.png" },
                { "relic_melted_icecream", "Assets/5.Sprites/3. Relics/5. 특수형/2. 녹아내린 아이스크림.png" },
                { "relic_skateboard", "Assets/5.Sprites/3. Relics/5. 특수형/3. 이상한 스케이트보드.png" },
                { "relic_twin_cherry", "Assets/5.Sprites/3. Relics/5. 특수형/4. 쌍둥이 체리.png" },
                { "relic_even_dice", "Assets/5.Sprites/3. Relics/5. 특수형/5. 짝수 주사위.png" },
                { "relic_odd_dice", "Assets/5.Sprites/3. Relics/5. 특수형/6. 홀수 주사위.png" },
                { "relic_strange_cloak", "Assets/5.Sprites/3. Relics/5. 특수형/7. 이상한 망토.png" },
                { "relic_jelly_bomb", "Assets/5.Sprites/3. Relics/5. 특수형/8. 젤리 폭탄.png" },
                { "relic_cursed_shield", "Assets/5.Sprites/3. Relics/5. 특수형/9. 저주받은 슬라임 방패.png" },
                { "relic_mirror", "Assets/5.Sprites/3. Relics/5. 특수형/10. 신비한 거울.png" }
            };
        }

        private static void AssignAttributeSprites(SerializedProperty listProperty, Dictionary<AttributeType, string> map)
        {
            listProperty.arraySize = map.Count;
            int index = 0;
            foreach (KeyValuePair<AttributeType, string> pair in map)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("AttributeType").enumValueIndex = (int)pair.Key;
                element.FindPropertyRelative("Sprite").objectReferenceValue = LoadSprite(pair.Value);
                index++;
            }
        }

        private static void AssignRelicSprites(SerializedProperty listProperty, Dictionary<string, string> map)
        {
            listProperty.arraySize = map.Count;
            int index = 0;
            foreach (KeyValuePair<string, string> pair in map)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("RelicId").stringValue = pair.Key;
                element.FindPropertyRelative("Sprite").objectReferenceValue = LoadSprite(pair.Value);
                index++;
            }
        }

        private static void AssignSprite(SerializedObject serializedObject, string propertyName, string assetPath, string spriteName = null)
        {
            serializedObject.FindProperty(propertyName).objectReferenceValue = LoadSprite(assetPath, spriteName);
        }

        private static Sprite LoadSprite(string assetPath, string spriteName = null)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool shouldReimport = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    shouldReimport = true;
                }

                SpriteImportMode expectedImportMode = string.IsNullOrWhiteSpace(spriteName) ? SpriteImportMode.Single : SpriteImportMode.Multiple;
                if (importer.spriteImportMode != expectedImportMode)
                {
                    importer.spriteImportMode = expectedImportMode;
                    shouldReimport = true;
                }

                if (importer.alphaIsTransparency == false)
                {
                    importer.alphaIsTransparency = true;
                    shouldReimport = true;
                }

                if (shouldReimport)
                {
                    importer.SaveAndReimport();
                }
            }

            Sprite sprite = null;
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (string.IsNullOrWhiteSpace(spriteName) == false)
            {
                for (int index = 0; index < assets.Length; index++)
                {
                    if (assets[index] is Sprite candidateSprite && candidateSprite.name == spriteName)
                    {
                        sprite = candidateSprite;
                        break;
                    }
                }
            }

            if (sprite == null)
            {
                for (int index = 0; index < assets.Length; index++)
                {
                    if (assets[index] is Sprite candidateSprite)
                    {
                        sprite = candidateSprite;
                        break;
                    }
                }
            }

            if (sprite == null)
            {
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            }

            if (sprite == null)
            {
                Debug.LogWarning($"스프라이트를 찾지 못했습니다: {assetPath} ({spriteName})");
            }

            return sprite;
        }

        private static void EnsureFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            string[] segments = assetFolderPath.Split('/');
            string currentPath = segments[0];

            for (int index = 1; index < segments.Length; index++)
            {
                string nextPath = $"{currentPath}/{segments[index]}";
                if (AssetDatabase.IsValidFolder(nextPath) == false)
                {
                    AssetDatabase.CreateFolder(currentPath, segments[index]);
                }

                currentPath = nextPath;
            }
        }
    }
}
