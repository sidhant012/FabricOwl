using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    public class RepairItem
    {
        public Scope Scope { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
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

        public string Kind { get; set; } = "RepairTask";

        //[JsonProperty("TaskId")]
        public string EventInstanceId { get; set; }

        public string Name { get; set; } = "";

        public string DataType { get; set; } = "RepairTask";

        public string TaskId { get; set; }

    }

    public class History
    {
        public DateTime CreatedUtcTimestamp { get; set; }
        public DateTime ClaimedUtcTimestamp { get; set; }
        public DateTime PreparingUtcTimestamp { get; set; }
        public DateTime ApprovedUtcTimestamp { get; set; }
        public DateTime ExecutingUtcTimestamp { get; set; }
        public DateTime RestoringUtcTimestamp { get; set; }
        public DateTime CompletedUtcTimestamp { get; set; }
        public DateTime PreparingHealthCheckStartUtcTimestamp { get; set; }
        public DateTime PreparingHealthCheckEndUtcTimestamp { get; set; }
        public DateTime RestoringHealthCheckStartUtcTimestamp { get; set; }
        public DateTime RestoringHealthCheckEndUtcTimestamp { get; set; }
    }

    public class Impact
    {
        public string Kind { get; set; }
        public List<NodeImpactList> NodeImpactList { get; set; }
    }

    public class NodeImpactList
    {
        public string NodeName { get; set; }
        public string ImpactLevel { get; set; }
    }

    public class Scope
    {
        public string Kind { get; set; }
    }

    public class Target
    {
        public string Kind { get; set; }
        public List<string> NodeNames { get; set; }
    }

}
