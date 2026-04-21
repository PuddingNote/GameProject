using System.Collections.Generic;
using SlimeExperiment.Battle;
using SlimeExperiment.Data;

namespace SlimeExperiment.Core
{
    public sealed class DraftService
    {
        private readonly PrototypeContentLibrary contentLibrary;
        private readonly IRandomSource randomSource;

        public DraftService(PrototypeContentLibrary contentLibrary, IRandomSource randomSource)
        {
            this.contentLibrary = contentLibrary;
            this.randomSource = randomSource;
        }

        public List<CardDefinition> DrawChoices(DraftPhase draftPhase)
        {
            IReadOnlyList<CardDefinition> sourceCards = GetSourceCards(draftPhase);
            List<CardDefinition> availableCards = new List<CardDefinition>(sourceCards);
            List<CardDefinition> result = new List<CardDefinition>();

            while (availableCards.Count > 0 && result.Count < 3)
            {
                int index = randomSource.NextInclusive(0, availableCards.Count - 1);
                result.Add(availableCards[index]);
                availableCards.RemoveAt(index);
            }

            return result;
        }

        public ExperimentBuildResult Assemble(CardDefinition attackCard, CardDefinition utilityCard, CardDefinition attributeCard)
        {
            return new ExperimentBuildResult
            {
                AttackCard = attackCard,
                UtilityCard = utilityCard,
                AttributeCard = attributeCard,
                MinAttack = attackCard.MinAttack,
                MaxAttack = attackCard.MaxAttack,
                AttributeType = attributeCard.AttributeType,
                UtilityEffectType = utilityCard.UtilityEffectType,
                UtilityChancePercent = utilityCard.UtilityChancePercent
            };
        }

        private IReadOnlyList<CardDefinition> GetSourceCards(DraftPhase draftPhase)
        {
            switch (draftPhase)
            {
                case DraftPhase.Attack:
                    return contentLibrary.AttackCards;
                case DraftPhase.Utility:
                    return contentLibrary.UtilityCards;
                case DraftPhase.Attribute:
                    return contentLibrary.AttributeCards;
                default:
                    return contentLibrary.AttackCards;
            }
        }
    }
}
