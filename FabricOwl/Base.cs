using FabricOwl.IConfigs;
using FabricOwl.Rules;
using FabricOwl.SFObjects;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Client.Http;
using Microsoft.ServiceFabric.Common.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security;

namespace FabricOwl
{
    public class Base
    {

        /*
         * Still ToDo
         *  - You could just filter the input events for the just the eventinstanceIds you need instead of filtering it out at the end so that your not doing an RCA for everything if you dont have to (this would be faster too) - Done
         */
        private static Dictionary<string, IPlugin> Plugins = new Dictionary<string, IPlugin>();
        private static string PluginPath = @"..\..\..\..\Plugins";

        private static string startTimeUTC = "2023-01-30T17:56:29Z";
        private static string endTimeUTC = "2023-02-01T18:53:13Z";
        //Another clusterURL to use once you have security credentials https://winlrc-sfrp-01.eastus.cloudapp.azure.com:19080

        public static void Main()
        {
            RCAEngine testRCA = new RCAEngine();
            string NodeData = "";
            string ApplicationData = "";
            string RepairTaskData = "";
            string ClusterData = "";

            Console.WriteLine("Test? (True/False): ");
            bool test = Convert.ToBoolean(Console.ReadLine());
            //Using this to check specific RCAs
            // some other IDs I can test s.InputEvent.EventInstanceId == "fcd49c38-cba6-4b76-be3f-4c8c337a3bed" (Node Deactivated --> due to Repair Task)
            // s.InputEvent.EventInstanceId == "80876de0-ae43-4ff0-be18-1070a68670b7" (Application Process Exited (APE) -->Node Down --> Node Deactivated --> due to Repair Task)
            // s.InputEvent.EventInstanceId == "0209c2ec-e9f8-425d-a332-7b4e65097134" (Node Down --> Node Deactivated --> due to Repair Task)
            // s.InputEvent.EventInstanceId == "2389d5b0-3fa0-4ab6-b64e-1555893ff38d" (Another APE event but self referential)
            Console.WriteLine("Enter EventInstanceId(s) and seperate IDs with a ',' (Do not enter anything if you want to see an RCA for all events): ");
            string eventInstanceIds = Console.ReadLine();
            eventInstanceIds = String.Concat(eventInstanceIds.Where(c => !Char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');

            Console.WriteLine("Enter the complete File Path for the json config you would like to use (Do not enter anything if you want to use the default config): ");
            string additionalConfig = Console.ReadLine();
            
            //generating the config to be used in RCA Engine
            IEnumerable<ConcurrentEventsConfig> testGenerateConfig = RelatedEventsConfigs.generateConfig();

            if (!String.IsNullOrEmpty(additionalConfig) && File.Exists(additionalConfig))
            {
                try
                {
                    string userConfig = File.ReadAllText(additionalConfig);
                    testGenerateConfig = JsonConvert.DeserializeObject<IEnumerable<ConcurrentEventsConfig>>(userConfig);

                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException)
                {
                    //This will cause FabricOwl to ternminate. Please check your config or access permissions and try again.
                    Console.WriteLine($"There is an issue with your inputed config: {e}");
                }
            }

            //If test is true then using test data otherwise using live cluster data
            if (test)
            {
                //reading in raw data files
                NodeData = File.ReadAllText(@"..\..\..\TestData\NodeEventsTestData.json");
                ApplicationData = File.ReadAllText(@"..\..\..\TestData\ApplicationEventsTestData.json");
                RepairTaskData = File.ReadAllText(@"..\..\..\TestData\RepairTasksTestData.json");
                ClusterData = File.ReadAllText(@"..\..\..\TestData\ClusterEventsTestData.json");

            } else
            {
                LoadPlugins();
                foreach(var key in Plugins.Keys)
                {
                    NodeData = Plugins[key].GetNodesEventList(startTimeUTC, endTimeUTC);
                    //ApplicationData = Plugins[key].GetApplicationsEventList(startTimeUTC, endTimeUTC);
                    //ClusterData = Plugins[key].GetClusterEventList(startTimeUTC, endTimeUTC);
                    //RepairTaskData = Plugins[key].GetRepairTasksEventList();
                }
            }

            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(NodeData);
            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(ApplicationData);
            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(RepairTaskData);
            var ClusterConvertEvents = JsonConvert.DeserializeObject<List<ClusterItem>>(ClusterData);
            RepairConvertEvents = SetRepairValues(RepairConvertEvents);


            //combining all the raw formatted data to a data type to be passed into the engine for RCA
            List<ICommonSFItems> inputEvents = new List<ICommonSFItems>();
            inputEvents.AddRange(NodeConvertEvents);
            inputEvents.AddRange(ApplicationConvertEvents);
            inputEvents.AddRange(RepairConvertEvents);
            inputEvents.AddRange(ClusterConvertEvents);

            
            List<ICommonSFItems> filteredInputEvents = new List<ICommonSFItems>();
            if(!String.IsNullOrEmpty(eventInstanceIds))
            {
                foreach (var inputEvent in inputEvents)
                {
                    foreach (var e in eventInstanceId)
                    {
                        if (!String.IsNullOrEmpty(e) && inputEvent.EventInstanceId == e)
                        {
                            filteredInputEvents.Add(inputEvent);
                        }
                    }
                }
            }

            //******** This is the actual execution of the RCA
            //******** Returns a list of the events and its respective RCA for each event
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            if (filteredInputEvents.Count > 0 )
            {
                simulEvents = testRCA.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            } else
            {
                simulEvents = testRCA.GetSimultaneousEventsForEvent(testGenerateConfig, inputEvents, inputEvents);
            }
           
            foreach(var s in simulEvents)
            {
                Console.WriteLine(JsonConvert.SerializeObject(s, Formatting.Indented));
            }
        }

        public static List<RepairItem> SetRepairValues(List<RepairItem> list)
        {
            foreach (var l in list)
            {
                l.EventInstanceId = l.TaskId;
                l.TimeStamp = l.History.CreatedUtcTimestamp;
            }
            return list;
        }

        public static void LoadPlugins()
        {
            if (!Path.IsPathRooted(PluginPath))
            {
                PluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + PluginPath;
            }
            foreach (var dll in Directory.GetFiles(PluginPath, "*.dll"))
            {
                AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext(dll);
                Assembly assembly = assemblyLoadContext.LoadFromAssemblyPath(dll);
                //var types = assembly.GetTypes();
                for(int i = 0; i < assembly.GetTypes().Length; i++)
                {
                    try
                    {
                        var plugin = Activator.CreateInstance(assembly.GetTypes()[i]) as IPlugin;
                        if (plugin is IPlugin)
                        {
                            Plugins.Add(Path.GetFileNameWithoutExtension(dll), plugin);
                        }
                    }
                    catch (MissingMethodException e)
                    {
                        continue;
                    }
                }
            }
        }
    }
}

