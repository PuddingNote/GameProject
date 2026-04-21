using System.Collections.Generic;
using System.IO;
using SlimeExperiment.Data;
using UnityEngine;

namespace SlimeExperiment.UI
{
    public static class PrototypeSpriteFallbackLibrary
    {
        private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

        public static Sprite GetCardFrameSprite()
        {
            return LoadSprite("5.Sprites/4. UI/1. 카드 테두리.png");
        }

        public static Sprite GetRelicFrameSprite()
        {
            return LoadSprite("5.Sprites/4. UI/2. 유물 테두리.png");
        }

        public static Sprite GetInfoFrameSprite()
        {
            return LoadSprite("5.Sprites/4. UI/3. 텍스트 테두리.png");
        }

        public static Sprite GetCharacterSprite(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Fire:
                    return LoadSprite("5.Sprites/1. Characters/2. 불속성 캐릭터.png");
                case AttributeType.Water:
                    return LoadSprite("5.Sprites/1. Characters/3. 물속성 캐릭터.png");
                case AttributeType.Grass:
                    return LoadSprite("5.Sprites/1. Characters/4. 풀속성 캐릭터.png");
                case AttributeType.Light:
                    return LoadSprite("5.Sprites/1. Characters/5. 빛속성 캐릭터.png");
                case AttributeType.Dark:
                    return LoadSprite("5.Sprites/1. Characters/6. 어둠속성 캐릭터.png");
                default:
                    return LoadSprite("5.Sprites/1. Characters/1. 무속성 캐릭터.png");
            }
        }

        public static Sprite GetCharacterHandSprite(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Fire:
                    return LoadSprite("5.Sprites/1. Characters/2. 불속성 캐릭터 손.png");
                case AttributeType.Water:
                    return LoadSprite("5.Sprites/1. Characters/3. 물속성 캐릭터 손.png");
                case AttributeType.Grass:
                    return LoadSprite("5.Sprites/1. Characters/4. 풀속성 캐릭터 손.png");
                case AttributeType.Light:
                    return LoadSprite("5.Sprites/1. Characters/5. 빛속성 캐릭터 손.png");
                case AttributeType.Dark:
                    return LoadSprite("5.Sprites/1. Characters/6. 어둠속성 캐릭터 손.png");
                default:
                    return LoadSprite("5.Sprites/1. Characters/1. 무속성 캐릭터 손.png");
            }
        }

        public static Sprite GetMonsterSprite(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Fire:
                    return LoadSprite("5.Sprites/2. Monsters/2. 불속성 몬스터.png");
                case AttributeType.Water:
                    return LoadSprite("5.Sprites/2. Monsters/3. 물속성 몬스터.png");
                case AttributeType.Grass:
                    return LoadSprite("5.Sprites/2. Monsters/4. 풀속성 몬스터.png");
                case AttributeType.Light:
                    return LoadSprite("5.Sprites/2. Monsters/5. 빛속성 몬스터.png");
                case AttributeType.Dark:
                    return LoadSprite("5.Sprites/2. Monsters/6. 어둠속성 몬스터.png");
                default:
                    return LoadSprite("5.Sprites/2. Monsters/1. 무속성 몬스터.png");
            }
        }

        public static Sprite GetRelicSprite(string relicId)
        {
            if (string.IsNullOrWhiteSpace(relicId))
            {
                return null;
            }

            switch (relicId)
            {
                case "relic_cotton_fist":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/1. 솜사탕 주먹.png");
                case "relic_pencil":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/2. 뾰족한 연필.png");
                case "relic_bat":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/3. 튼튼한 나무방망이.png");
                case "relic_slime_mucus":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/4. 슬라임 점액.png");
                case "relic_bear":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/5. 거대 곰인형.png");
                case "relic_jelly":
                    return LoadSprite("5.Sprites/3. Relics/1. 기본 스탯형/6. 달콤한 젤리.png");
                case "relic_clockwork":
                    return LoadSprite("5.Sprites/3. Relics/2. 확률 강화형/1. 태엽 장치.png");
                case "relic_pinwheel":
                    return LoadSprite("5.Sprites/3. Relics/2. 확률 강화형/2. 바람개비 모자.png");
                case "relic_soft_jelly":
                    return LoadSprite("5.Sprites/3. Relics/2. 확률 강화형/3. 말랑한 젤리.png");
                case "relic_stardust":
                    return LoadSprite("5.Sprites/3. Relics/2. 확률 강화형/4. 반짝이는 별가루.png");
                case "relic_bomb":
                    return LoadSprite("5.Sprites/3. Relics/2. 확률 강화형/5. 째깍거리는 시한폭탄.png");
                case "relic_broken_toy":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/1. 부서진 장난감.png");
                case "relic_coward_cloak":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/2. 겁쟁이 망토.png");
                case "relic_surprise_box":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/3. 깜짝상자.png");
                case "relic_combo_balloon":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/4. 콤보 풍선.png");
                case "relic_ogre_mask":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/5. 화난 도깨비가면.png");
                case "relic_sticky_sword":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/6. 끈적끈적한 검.png");
                case "relic_badge":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/7. 대장님 배지.png");
                case "relic_bouncy_ball":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/8. 탱탱볼.png");
                case "relic_bubble":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/9. 비눗방울.png");
                case "relic_piggy_bank":
                    return LoadSprite("5.Sprites/3. Relics/3. 조건형/10. 저주받은 저금통.png");
                case "relic_rainbow":
                    return LoadSprite("5.Sprites/3. Relics/4. 속성 시너지형/1. 무지개 사탕.png");
                case "relic_clear_cloak":
                    return LoadSprite("5.Sprites/3. Relics/4. 속성 시너지형/2. 투명망토.png");
                case "relic_gray_mucus":
                    return LoadSprite("5.Sprites/3. Relics/4. 속성 시너지형/3. 회색 슬라임 점액.png");
                case "relic_cursed_hammer":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/1. 저주받은 망치.png");
                case "relic_melted_icecream":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/2. 녹아내린 아이스크림.png");
                case "relic_skateboard":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/3. 이상한 스케이트보드.png");
                case "relic_twin_cherry":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/4. 쌍둥이 체리.png");
                case "relic_even_dice":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/5. 짝수 주사위.png");
                case "relic_odd_dice":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/6. 홀수 주사위.png");
                case "relic_strange_cloak":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/7. 이상한 망토.png");
                case "relic_jelly_bomb":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/8. 젤리 폭탄.png");
                case "relic_cursed_shield":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/9. 저주받은 슬라임 방패.png");
                case "relic_mirror":
                    return LoadSprite("5.Sprites/3. Relics/5. 특수형/10. 신비한 거울.png");
                default:
                    return null;
            }
        }

        private static Sprite LoadSprite(string relativeAssetPath)
        {
            if (SpriteCache.TryGetValue(relativeAssetPath, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            string absolutePath = Path.Combine(Application.dataPath, relativeAssetPath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(absolutePath) == false)
            {
                SpriteCache[relativeAssetPath] = null;
                return null;
            }

            byte[] imageData = File.ReadAllBytes(absolutePath);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(imageData) == false)
            {
                Object.Destroy(texture);
                SpriteCache[relativeAssetPath] = null;
                return null;
            }

            texture.name = Path.GetFileNameWithoutExtension(relativeAssetPath);
            texture.filterMode = FilterMode.Bilinear;
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            sprite.name = texture.name;
            SpriteCache[relativeAssetPath] = sprite;
            return sprite;
        }
    }
}
