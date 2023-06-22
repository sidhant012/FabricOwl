using FabricOwl.IConfigs;
using Newtonsoft.Json;
using System;

namespace FabricOwl.SFObjects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ClusterItem : IClusterItem
    {
        public string TargetClusterVersion { get; set; }
        public double OverallUpgradeElapsedTimeInMs { get; set; }
        public string Kind { get; set; }
        public string EventInstanceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
        public string UpgradeState { get; set; }
        public string UpgradeDomains { get; set; }
        public double? UpgradeDomainElapsedTimeInMs { get; set; }
        public string SourceId { get; set; }
        public string Property { get; set; }
        public string HealthState { get; set; }
        public long? TimeToLiveMs { get; set; }
        public long? SequenceNumber { get; set; }
        public string Description { get; set; }
        public bool? RemoveWhenExpired { get; set; }
        public DateTime? SourceUtcTimestamp { get; set; }
        public string CurrentClusterVersion { get; set; }
        public string UpgradeType { get; set; }
        public string RollingUpgradeMode { get; set; }
        public string FailureAction { get; set; }
        public string DataType { get; set; } = "Cluster";
    }
}
