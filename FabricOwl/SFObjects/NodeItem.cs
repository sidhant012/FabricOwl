using FabricOwl.IConfigs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.SFObjects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NodeItem : INodeItem
    {
        public string NodeInstanceId { get; set; }
        public string SourceId { get; set; }
        public string Property { get; set; }
        public string HealthState { get; set; }
        public string TimeToLiveMs { get; set; }
        public string SequenceNumber { get; set; }
        public string Description { get; set; }
        public string RemoveWhenExpired { get; set; }
        public DateTime SourceUtcTimestamp { get; set; }
        public string NodeName { get; set; }
        public string Kind { get; set; }
        public string EventInstanceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
        public string NodeInstance { get; set; }
        public DateTime? LastNodeDownAt { get; set; }
        public string NodeId { get; set; }
        public string UpgradeDomain { get; set; }
        public string FaultDomain { get; set; }
        public string IpAddressOrFQDN { get; set; }
        public string Hostname { get; set; }
        public string IsSeedNode { get; set; }
        public string NodeVersion { get; set; }
        public string EffectiveDeactivateIntent { get; set; }
        public string BatchIdsWithDeactivateIntent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? LastNodeUpAt { get; set; }
        public string Error { get; set; }
        public string BatchId { get; set; }
        public string DeactivateIntent { get; set; }
        public string DataType { get; set; } = "Node";
    }
}
