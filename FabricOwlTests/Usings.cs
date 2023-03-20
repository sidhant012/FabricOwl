global using Microsoft.VisualStudio.TestTools.UnitTesting;
using FabricOwl.IConfigs;
using FabricOwl.SFObjects;
using Newtonsoft.Json;

namespace FabricOwlTests
{
    public class Usings
    {
        //Using this to check specific RCAs
        // some other IDs I can test s.InputEvent.EventInstanceId == "fcd49c38-cba6-4b76-be3f-4c8c337a3bed" (Node Deactivated --> due to Repair Task)
        // s.InputEvent.EventInstanceId == "80876de0-ae43-4ff0-be18-1070a68670b7" (Application Process Exited (APE) -->Node Down --> Node Deactivated --> due to Repair Task)
        // s.InputEvent.EventInstanceId == "0209c2ec-e9f8-425d-a332-7b4e65097134" (Node Down --> Node Deactivated --> due to Repair Task)
        // s.InputEvent.EventInstanceId == "2389d5b0-3fa0-4ab6-b64e-1555893ff38d" (Another APE event but self referential)
        // s.InputEvent.EventInstanceId == "f01732cf-092e-4fcc-b174-a85b03345d30" (PartitionReconfigurationStarted)
        // s.InputEvent.EventInstanceId == "5300a654-9ff0-40c7-8a31-4ab6dc5ed755" (ClusterHealthReport --> Node Closed --> Node Down)
        public List<ICommonSFItems> getInputEvents()
        {
            List<ICommonSFItems> inputEvents = new List<ICommonSFItems>();

            //reading in raw data files
            var NodeData = File.ReadAllText(@"..\..\..\TestData\NodeEventsTestData.json");
            var ApplicationData = File.ReadAllText(@"..\..\..\TestData\ApplicationEventsTestData.json");
            var RepairTaskData = File.ReadAllText(@"..\..\..\TestData\RepairTasksTestData.json");
            var ClusterData = File.ReadAllText(@"..\..\..\TestData\ClusterEventsTestData.json");
            var PartitionData = File.ReadAllText(@"..\..\..\TestData\PartitionEventsTestData.json");

            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(NodeData);
            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(ApplicationData);
            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(RepairTaskData);
            var ClusterConvertEvents = JsonConvert.DeserializeObject<List<ClusterItem>>(ClusterData);
            var PartitionConvertEvents = JsonConvert.DeserializeObject<List<PartitionItem>>(PartitionData);
            RepairConvertEvents = SetRepairValues(RepairConvertEvents);

            inputEvents.AddRange(NodeConvertEvents);
            inputEvents.AddRange(ApplicationConvertEvents);
            inputEvents.AddRange(RepairConvertEvents);
            inputEvents.AddRange(ClusterConvertEvents);
            inputEvents.AddRange(PartitionConvertEvents);

            return inputEvents;

        }

        public List<RepairItem> SetRepairValues(List<RepairItem> list)
        {
            foreach (var l in list)
            {
                l.EventInstanceId = l.TaskId;
                l.TimeStamp = l.History.CreatedUtcTimestamp;
            }
            return list;
        }
    }
}