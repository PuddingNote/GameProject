using System;
using System.IO;
using System.Collections.Generic;
using SlimeExperiment.Data;
using UnityEngine;

namespace SlimeExperiment.Logbook
{
    public sealed class ExperimentLogService
    {
        private readonly string filePath;
        private readonly PrototypeContentLibrary contentLibrary;
        private ExperimentRecordCollection cachedCollection;

        public ExperimentLogService(PrototypeContentLibrary contentLibrary)
        {
            this.contentLibrary = contentLibrary;
            filePath = Path.Combine(Application.persistentDataPath, "slime_experiment_log.json");
            cachedCollection = LoadCollection();
            EnsureDiscoveredRelicsBackfilled();
        }

        public int GetNextExperimentNumber()
        {
            return cachedCollection.Records.Count + 1;
        }

        public void SaveRecord(ExperimentRecord record)
        {
            cachedCollection.Records.Add(record);
            SaveCollection();
        }

        public void MarkRelicDiscovered(string relicId)
        {
            if (string.IsNullOrWhiteSpace(relicId))
            {
                return;
            }

            if (cachedCollection.DiscoveredRelicIds.Contains(relicId) == false)
            {
                cachedCollection.DiscoveredRelicIds.Add(relicId);
                SaveCollection();
            }
        }

        public bool IsRelicDiscovered(string relicId)
        {
            if (string.IsNullOrWhiteSpace(relicId))
            {
                return false;
            }

            return cachedCollection.DiscoveredRelicIds.Contains(relicId);
        }

        public List<string> GetDiscoveredRelicIds()
        {
            return new List<string>(cachedCollection.DiscoveredRelicIds);
        }

        public List<ExperimentRecord> GetRecords()
        {
            return new List<ExperimentRecord>(cachedCollection.Records);
        }

        private ExperimentRecordCollection LoadCollection()
        {
            if (File.Exists(filePath) == false)
            {
                return new ExperimentRecordCollection();
            }

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new ExperimentRecordCollection();
            }

            ExperimentRecordCollection loaded = JsonUtility.FromJson<ExperimentRecordCollection>(json);
            if (loaded == null)
            {
                return new ExperimentRecordCollection();
            }

            if (loaded.Records == null)
            {
                loaded.Records = new List<ExperimentRecord>();
            }

            if (loaded.DiscoveredRelicIds == null)
            {
                loaded.DiscoveredRelicIds = new List<string>();
            }

            for (int index = 0; index < loaded.Records.Count; index++)
            {
                ExperimentRecord record = loaded.Records[index];
                if (record == null)
                {
                    continue;
                }

                if (record.SelectedCards == null)
                {
                    record.SelectedCards = new List<string>();
                }

                if (record.SelectedCardEffects == null)
                {
                    record.SelectedCardEffects = new List<string>();
                }

                if (record.AcquiredRelics == null)
                {
                    record.AcquiredRelics = new List<string>();
                }

                if (record.AcquiredRelicIds == null)
                {
                    record.AcquiredRelicIds = new List<string>();
                }
            }

            return loaded;
        }

        private void EnsureDiscoveredRelicsBackfilled()
        {
            if (contentLibrary == null)
            {
                return;
            }

            bool hasChanges = false;
            for (int recordIndex = 0; recordIndex < cachedCollection.Records.Count; recordIndex++)
            {
                ExperimentRecord record = cachedCollection.Records[recordIndex];
                if (record == null)
                {
                    continue;
                }

                if (record.SelectedCardEffects == null)
                {
                    record.SelectedCardEffects = new List<string>();
                }

                if (record.SelectedCardEffects.Count == 0 && record.SelectedCards.Count > 0)
                {
                    for (int cardIndex = 0; cardIndex < record.SelectedCards.Count; cardIndex++)
                    {
                        string cardEffect = FindCardDescriptionByDisplayName(record.SelectedCards[cardIndex]);
                        if (string.IsNullOrWhiteSpace(cardEffect) == false)
                        {
                            record.SelectedCardEffects.Add(cardEffect);
                            hasChanges = true;
                        }
                    }
                }

                for (int relicIndex = 0; relicIndex < record.AcquiredRelicIds.Count; relicIndex++)
                {
                    string relicId = record.AcquiredRelicIds[relicIndex];
                    if (string.IsNullOrWhiteSpace(relicId) || cachedCollection.DiscoveredRelicIds.Contains(relicId))
                    {
                        continue;
                    }

                    cachedCollection.DiscoveredRelicIds.Add(relicId);
                    hasChanges = true;
                }

                for (int relicIndex = 0; relicIndex < record.AcquiredRelics.Count; relicIndex++)
                {
                    string relicName = record.AcquiredRelics[relicIndex];
                    string relicId = FindRelicIdByDisplayName(relicName);
                    if (string.IsNullOrWhiteSpace(relicId) || cachedCollection.DiscoveredRelicIds.Contains(relicId))
                    {
                        continue;
                    }

                    cachedCollection.DiscoveredRelicIds.Add(relicId);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                SaveCollection();
            }
        }

        private string FindRelicIdByDisplayName(string displayName)
        {
            if (contentLibrary == null || string.IsNullOrWhiteSpace(displayName))
            {
                return string.Empty;
            }

            IReadOnlyList<RelicDefinition> relics = contentLibrary.Relics;
            for (int index = 0; index < relics.Count; index++)
            {
                if (string.Equals(relics[index].DisplayName, displayName, StringComparison.Ordinal))
                {
                    return relics[index].Id;
                }
            }

            return string.Empty;
        }

        private string FindCardDescriptionByDisplayName(string displayName)
        {
            if (contentLibrary == null || string.IsNullOrWhiteSpace(displayName))
            {
                return string.Empty;
            }

            string description = FindCardDescriptionInList(contentLibrary.AttackCards, displayName);
            if (string.IsNullOrWhiteSpace(description) == false)
            {
                return description;
            }

            description = FindCardDescriptionInList(contentLibrary.UtilityCards, displayName);
            if (string.IsNullOrWhiteSpace(description) == false)
            {
                return description;
            }

            return FindCardDescriptionInList(contentLibrary.AttributeCards, displayName);
        }

        private static string FindCardDescriptionInList(IReadOnlyList<CardDefinition> cards, string displayName)
        {
            for (int index = 0; index < cards.Count; index++)
            {
                if (string.Equals(cards[index].DisplayName, displayName, StringComparison.Ordinal))
                {
                    return cards[index].Description;
                }
            }

            return string.Empty;
        }

        private void SaveCollection()
        {
            string json = JsonUtility.ToJson(cachedCollection, true);
            File.WriteAllText(filePath, json);
        }
    }
}
