using FabricOwl.IConfigs;
using Newtonsoft.Json;
using System;

namespace FabricOwl.SFObjects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PartitionItem : IPartitionItem
    {
        public string SourceId { get; set; }
        public string Property { get; set; }
        public string HealthState { get; set; }
        public long TimeToLiveMs { get; set; }
        public object SequenceNumber { get; set; }
        public string Description { get; set; }
        public bool RemoveWhenExpired { get; set; }
        public DateTime SourceUtcTimestamp { get; set; }
        public string PartitionId { get; set; }
        public string Kind { get; set; }
        public string EventInstanceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Category { get; set; }
        public bool HasCorrelatedEvents { get; set; }
        public string NodeName { get; set; }
        public string NodeInstanceId { get; set; }
        public string ServiceType { get; set; }
        public long? CcEpochDataLossVersion { get; set; }
        public long? CcEpochConfigVersion { get; set; }
        public string ReconfigType { get; set; }
        public string Result { get; set; }
        public double? Phase0DurationMs { get; set; }
        public double? Phase1DurationMs { get; set; }
        public double? Phase2DurationMs { get; set; }
        public double? Phase3DurationMs { get; set; }
        public double? Phase4DurationMs { get; set; }
        public double? TotalDurationMs { get; set; }
        public string ActivityId { get; set; }
        public string OldPrimaryNodeName { get; set; }
        public string OldPrimaryNodeId { get; set; }
        public string OldSecondaryNodeNames { get; set; }
        public string OldSecondaryNodeIds { get; set; }
        public string NewPrimaryNodeName { get; set; }
        public string NewPrimaryNodeId { get; set; }
        public string NewSecondaryNodeNames { get; set; }
        public string NewSecondaryNodeIds { get; set; }
        public string DecisionId { get; set; }
        public string Phase { get; set; }
        public string Action { get; set; }
        public string Service { get; set; }
        public string SourceNode { get; set; }
        public string TargetNode { get; set; }
        public string MoveCost { get; set; }
        public string DataType { get; set; } = "Partition";
    }
}
