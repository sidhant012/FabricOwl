using AutoMapper;
using FabricOwl.IConfigs;
using FabricOwl.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FabricOwl
{
    public class Base
    {

        /*
         * Still ToDo
         *  - Need to add a better way to read in the data (the current implementation is for simplicity purposes to make sure everything is working) ---> Will use Get Requests to the EventStore to retrive data https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-index-eventsstore
         */

        public static void Main()
        {
            RCAEngine testRCA = new RCAEngine();
            InputDataProperties inputData = new InputDataProperties();
            
            //generating the config to be used in RCA Engine
            IEnumerable<ConcurrentEventsConfig> testGenerateConfig = RelatedEventsConfigs.generateConfig();

            //reading in raw data files
            string NodeData = File.ReadAllText("C:\\Users\\sibhatia\\source\\repos\\FabricOwl\\FabricOwl\\TestData\\NodeEventsTestData.json");
            string ApplicationData = File.ReadAllText("C:\\Users\\sibhatia\\source\\repos\\FabricOwl\\FabricOwl\\TestData\\ApplicationEventsTestData.json");
            string RepairTaskData = File.ReadAllText("C:\\Users\\sibhatia\\source\\repos\\FabricOwl\\FabricOwl\\TestData\\RepairTasksTestData.json");

            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(NodeData);
            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(ApplicationData);
            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(RepairTaskData);

            //combining all the raw formatted data to a data type to be passed into the engine for RCA
            List<CombinedSFItems> inputEvents = new List<CombinedSFItems>();
            inputEvents = NodeToGenericItem(NodeConvertEvents, inputEvents);
            inputEvents = ApplicationToGenericItem(ApplicationConvertEvents, inputEvents);
            inputEvents = RepairToGenericItem(RepairConvertEvents, inputEvents);

            //Console.WriteLine(JsonConvert.SerializeObject(inputEvents, Formatting.Indented));

/*            //Converting raw data to a format that can be used in the RCA engine
            var NodeConvert = JsonConvert.DeserializeObject(NodeData);
            NodeConvert = inputData.EventStorePropertiesTransformations(NodeConvert);
            var ApplicationConvert = JsonConvert.DeserializeObject(ApplicationData);
            ApplicationConvert = inputData.EventStorePropertiesTransformations(ApplicationConvert);
            var RepairConvert = JsonConvert.DeserializeObject(RepairTaskData);
            RepairConvert = inputData.RepairTasksPropertiesTransformations(RepairConvert);

            var allData = new List<object>()
            { NodeConvert, ApplicationConvert, RepairConvert};
            //combining all the formatted data to be passed into the engine
            var inputEvents = inputData.CombineData(allData);
            //Console.WriteLine(JsonConvert.SerializeObject(inputEvents, Formatting.Indented));*/

            //******** This is the actual execution of the RCA
            //******** Returns a list of the events and its respective RCA for each event
            List<ConcurrentEvents> simulEvents = testRCA.GetSimultaneousEventsForEvent(testGenerateConfig, inputEvents, inputEvents);
            //Console.WriteLine("Result: " + JsonConvert.SerializeObject(simulEvents, Formatting.Indented));
           
            //This is for testing purposes Im choosing specific IDs to check if the RCA is correct given the data provided
            List<ConcurrentEvents> tester = new List<ConcurrentEvents>();
            foreach(var s in simulEvents)
            {
                //Using this to check specific RCAs
                // some other IDs I can test s.InputEvent.EventInstanceId == "fcd49c38-cba6-4b76-be3f-4c8c337a3bed" (Node Deactivated --> due to Repair Task)
                // s.InputEvent.EventInstanceId == "80876de0-ae43-4ff0-be18-1070a68670b7" (Application Process Exited (APE) -->Node Down --> Node Deactivated --> due to Repair Task)
                // s.InputEvent.EventInstanceId == "0209c2ec-e9f8-425d-a332-7b4e65097134" (Node Down --> Node Deactivated --> due to Repair Task)
                // s.InputEvent.EventInstanceId == "2389d5b0-3fa0-4ab6-b64e-1555893ff38d" (Another APE event but self referential)
                if (s.InputEvent.EventInstanceId == "80876de0-ae43-4ff0-be18-1070a68670b7")
                {
                    tester.Add(s);
                }
            }

            foreach(var t in tester)
            {
                Console.WriteLine(JsonConvert.SerializeObject(t, Formatting.Indented));
            }

        }

        public static List<CombinedSFItems> NodeToGenericItem(List<NodeItem> nodeEvents, List<CombinedSFItems> inputEvents)
        {
            var nodeConfig = new MapperConfiguration(cfg => cfg.CreateMap<NodeItem, CombinedSFItems>());
            var mapper = new Mapper(nodeConfig);

            foreach(var node in nodeEvents)
            {
                var input = mapper.Map<CombinedSFItems>(node);
                inputEvents.Add(input);
            }

            return inputEvents;
        }

        public static List<CombinedSFItems> ApplicationToGenericItem(List<ApplicationItem> applicationEvents, List<CombinedSFItems> inputEvents)
        {
            var applicationConfig = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationItem, CombinedSFItems>());
            var mapper = new Mapper(applicationConfig);

            foreach (var application in applicationEvents)
            {
                var input = mapper.Map<CombinedSFItems>(application);
                inputEvents.Add(input);
            }

            return inputEvents;
        }

        public static List<CombinedSFItems> RepairToGenericItem(List<RepairItem> repairEvents, List<CombinedSFItems> inputEvents)
        {
            var repairConfig = new MapperConfiguration(cfg => cfg.CreateMap<RepairItem, CombinedSFItems>());
            var mapper = new Mapper(repairConfig);

            foreach (var repair in repairEvents)
            {
                repair.EventInstanceId = repair.TaskId;
                var input = mapper.Map<CombinedSFItems>(repair);
                inputEvents.Add(input);
            }

            return inputEvents;
        }
    }
}

