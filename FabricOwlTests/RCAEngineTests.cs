﻿using FabricOwl;
using FabricOwl.IConfigs;
using FabricOwl.Rules;
using FabricOwl.SFObjects;
using Newtonsoft.Json;

namespace FabricOwlTests
{
    [TestClass]
    public class RCAEngineTests
    {
        RCAEngine rca = new RCAEngine();
        static string config = File.ReadAllText(@"..\..\..\ConfigData\generatedConfig.txt");
        IEnumerable<ConcurrentEventsConfig> testGenerateConfig = JsonConvert.DeserializeObject<IEnumerable<ConcurrentEventsConfig>>(config);

        [TestMethod]
        public void Test_self_APE()
        {
            string eventInstanceId = "2389d5b0-3fa0-4ab6-b64e-1555893ff38d";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_APE_self.txt"));
        }

        [TestMethod]
        public void Test_NodeDeactivated()
        {
            string eventInstanceId = "fcd49c38-cba6-4b76-be3f-4c8c337a3bed";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_NodeDeactivated.txt"));
        }

        [TestMethod]
        public void Test_APE_RepairTask()
        {
            string eventInstanceId = "80876de0-ae43-4ff0-be18-1070a68670b7";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_APE_RepairTask.txt"));

        }

        [TestMethod]
        public void Test_NodeDown_RepairTask()
        {
            string eventInstanceId = "0209c2ec-e9f8-425d-a332-7b4e65097134";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_NodeDown_RepairTask.txt"));
        }

        [TestMethod]
        public void Test_PartitionReconfigured()
        {
            string eventInstanceId = "f01732cf-092e-4fcc-b174-a85b03345d30";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_PartitionReconfigured.txt"));
        }

        [TestMethod]
        public void Test_ClusterReport_NodeDown()
        {
            string eventInstanceId = "5300a654-9ff0-40c7-8a31-4ab6dc5ed755";
            List<ICommonSFItems> inputEvents = getInputEvents();
            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(inputEvents, eventInstanceId);
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = "";
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"..\..\..\RCAEngineOutputs\RCA_ClusterReport_NodeDown.txt"));
        }

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

        public List<ICommonSFItems> getFilteredInputEvents(List<ICommonSFItems> inputEvents, string eventInstanceId)
        {
            List<ICommonSFItems> filteredInputEvents = new List<ICommonSFItems>();
            foreach (var inputEvent in inputEvents)
            {
                if(inputEvent.EventInstanceId == eventInstanceId)
                {
                    filteredInputEvents.Add(inputEvent);
                }
            }

            return filteredInputEvents;
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
