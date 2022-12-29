using FabricOwl.IConfigs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CombinedSFItems
    {
        //Items that are listed in more than one item
        public string Description { get; set; }
        public string Kind { get; set; }
        public string EventInstanceId { get; set; }
        public string DataType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
        public string NodeInstance { get; set; }
        public DateTime? LastNodeUpAt { get; set; }
        public string NodeName { get; set; }



        //Node Items
        public string NodeInstanceId { get; set; }
        public string SourceId { get; set; }
        public string Property { get; set; }
        public string HealthState { get; set; }
        public string TimeToLiveMs { get; set; }
        public string SequenceNumber { get; set; }
        public string RemoveWhenExpired { get; set; }
        public DateTime SourceUtcTimestamp { get; set; }
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
        public string Error { get; set; }
        public string BatchId { get; set; }
        public string DeactivateIntent { get; set; }

        
        //Repair Items
        public Scope Scope { get; set; }
        public string Version { get; set; }
        public string State { get; set; }
        public string Flags { get; set; }
        public string Action { get; set; }
        public Target Target { get; set; }
        public string Executor { get; set; }
        public string ExecutorData { get; set; }
        public Impact Impact { get; set; }
        public string ResultStatus { get; set; }
        public string ResultCode { get; set; }
        public string ResultDetails { get; set; }
        public History History { get; set; }
        public string PreparingHealthCheckState { get; set; }
        public string RestoringHealthCheckState { get; set; }
        public string PerformPreparingHealthCheck { get; set; }
        public string PerformRestoringHealthCheck { get; set; }

        public string Name { get; set; } = "";
        public string TaskId { get; set; }



        //Application Items
        public string ServiceName { get; set; }
        public string ServicePackageName { get; set; }
        public string ServicePackageActivationId { get; set; }
        public string IsExclusive { get; set; }
        public string CodePackageName { get; set; }
        public string EntryPointType { get; set; }
        public string ExeName { get; set; }
        public string ProcessId { get; set; }
        public string HostId { get; set; }
        public string ExitCode { get; set; }
        public string UnexpectedTermination { get; set; }
        public string ActivityId { get; set; }
        public string ExitReason { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ApplicationTypeVersion { get; set; }
        public string OverallUpgradeElapsedTimeInMs { get; set; }
        public string CurrentApplicationTypeVersion { get; set; }
        public string UpgradeState { get; set; }
        public string UpgradeDomains { get; set; }
        public string UpgradeDomainElapsedTimeInMs { get; set; }
        public string UpgradeType { get; set; }
        public string RollingUpgradeMode { get; set; }
        public string FailureAction { get; set; }
        public string ImageName { get; set; }
        public string ContainerName { get; set; }
    }
}
