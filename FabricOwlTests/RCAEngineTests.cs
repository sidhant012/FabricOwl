using FabricOwl;
using FabricOwl.IConfigs;
using FabricOwl.Rules;
using Newtonsoft.Json;

namespace FabricOwlTests
{
    [TestClass]
    public class RCAEngineTests
    {
        private readonly RCAEngine rca = new();
        private static readonly string config = File.ReadAllText(@"ConfigData\generatedConfig.txt");
        private IEnumerable<ConcurrentEventsConfig>? testGenerateConfig = null;

        [TestInitialize]
        public void InitTests()
        {
            Assert.IsTrue(JsonHelper.TryDeserializeObject(config, out testGenerateConfig, true));
        }
        
        [TestMethod]
        public void Test_self_APE()
        {
            string eventInstanceIds = "2389d5b0-3fa0-4ab6-b64e-1555893ff38d";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_APE_self.txt"));
        }

        [TestMethod]
        public void Test_NodeDeactivated()
        {
            string eventInstanceIds = "fcd49c38-cba6-4b76-be3f-4c8c337a3bed";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_NodeDeactivated.txt"));
        }

        [TestMethod]
        public void Test_APE_RepairTask()
        {
            string eventInstanceIds = "80876de0-ae43-4ff0-be18-1070a68670b7";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_APE_RepairTask.txt"));

        }

        [TestMethod]
        public void Test_NodeDown_RepairTask()
        {
            string eventInstanceIds = "0209c2ec-e9f8-425d-a332-7b4e65097134";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_NodeDown_RepairTask.txt"));
        }

        [TestMethod]
        public void Test_PartitionReconfigured()
        {
            string eventInstanceIds = "f01732cf-092e-4fcc-b174-a85b03345d30";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_PartitionReconfigured.txt"));
        }

        [TestMethod]
        public void Test_ClusterReport_NodeDown()
        {
            string eventInstanceIds = "5300a654-9ff0-40c7-8a31-4ab6dc5ed755";
            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = Usings.GetInputEvents();
            List<ICommonSFItems> filteredInputEvents = Base.GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            List<RCAEvents> simulEvents = new();
            simulEvents = rca.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            string result = string.Empty;
            foreach (var simulEvent in simulEvents)
            {
                result += JsonConvert.SerializeObject(simulEvent, Formatting.Indented);
            }
            Assert.AreEqual(result, File.ReadAllText(@"RCAEngineOutputs\RCA_ClusterReport_NodeDown.txt"));
        }
    }
}
