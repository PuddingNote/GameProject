using System;
using System.Collections.Generic;

namespace SlimeExperiment.Logbook
{
    [Serializable]
    public sealed class ExperimentRecord
    {
        public int ExperimentNumber;
        public bool Succeeded;
        public int ReachedStage;
        public List<string> SelectedCards = new List<string>();
        public List<string> SelectedCardEffects = new List<string>();
        public List<string> AcquiredRelics = new List<string>();
        public List<string> AcquiredRelicIds = new List<string>();
        public string ResultSummary;
        public string CreatedAt;
    }

    [Serializable]
    public sealed class ExperimentRecordCollection
    {
        public List<ExperimentRecord> Records = new List<ExperimentRecord>();
        public List<string> DiscoveredRelicIds = new List<string>();
    }
}
