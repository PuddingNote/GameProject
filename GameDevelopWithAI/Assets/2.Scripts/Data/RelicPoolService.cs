using System.Collections.Generic;

namespace SlimeExperiment.Data
{
    public sealed class RelicPoolService
    {
        private readonly PrototypeContentLibrary contentLibrary;

        public RelicPoolService(PrototypeContentLibrary contentLibrary)
        {
            this.contentLibrary = contentLibrary;
        }

        public List<RelicDefinition> GetEligibleRelics(IReadOnlyList<RelicDefinition> ownedRelics)
        {
            List<RelicDefinition> eligibleRelics = new List<RelicDefinition>();

            for (int index = 0; index < contentLibrary.Relics.Count; index++)
            {
                RelicDefinition candidate = contentLibrary.Relics[index];
                if (ContainsRelic(ownedRelics, candidate.Id))
                {
                    continue;
                }

                if (IsBlockedByOwnedRelic(ownedRelics, candidate))
                {
                    continue;
                }

                eligibleRelics.Add(candidate);
            }

            return eligibleRelics;
        }

        private static bool ContainsRelic(IReadOnlyList<RelicDefinition> ownedRelics, string relicId)
        {
            for (int index = 0; index < ownedRelics.Count; index++)
            {
                if (ownedRelics[index].Id == relicId)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBlockedByOwnedRelic(IReadOnlyList<RelicDefinition> ownedRelics, RelicDefinition candidate)
        {
            for (int index = 0; index < ownedRelics.Count; index++)
            {
                RelicDefinition ownedRelic = ownedRelics[index];
                if (ownedRelic.BlocksRelicId == candidate.Id)
                {
                    return true;
                }

                if (candidate.BlocksRelicId == ownedRelic.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
