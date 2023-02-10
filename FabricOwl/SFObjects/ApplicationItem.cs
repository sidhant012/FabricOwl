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
    public class ApplicationItem : ICommonSFItems
    {
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
        public DateTime StartTime { get; set; }
        public string ActivityId { get; set; }
        public string ExitReason { get; set; }
        public string ApplicationId { get; set; }
        public string Kind { get; set; }
        public string EventInstanceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
        public string NodeInstance { get; set; }
        public DateTime? LastNodeUpAt { get; set; }
        public string NodeName { get; set; }
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
        public string DataType { get; set; } = "Application";
    }
}
