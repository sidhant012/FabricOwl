using FabricOwl.IConfigs;
using FabricOwl.Rules;
using FabricOwl.SFObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Security;

namespace FabricOwl
{
    public class Base
    {

        /*
         * Still ToDo
         *  
         */
        protected static Dictionary<string, IPlugin> Plugins = new Dictionary<string, IPlugin>();
        private static string PluginPath = @"..\..\..\..\Plugins";

        private static string startTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow.AddDays(-7));
        private static string endTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow);
        static Boolean load = true;
        //Another clusterURL to use once you have security credentials https://winlrc-sfrp-01.eastus.cloudapp.azure.com:19080

        public static void Main()
        {
            RCAEngine testRCA = new RCAEngine();

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

            List<ICommonSFItems> inputEvents = new List<ICommonSFItems>();

            LoadPlugins();
            foreach (var key in Plugins.Keys)
            {
                inputEvents = Plugins[key].ReturnEvents(inputEvents, startTimeUTC, endTimeUTC);   
            }

            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);

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

        public List<RCAEvents> getRCA(string eventInstanceIds = "")
        {
            RCAEngine testRCA = new RCAEngine();

            eventInstanceIds = String.Concat(eventInstanceIds.Where(c => !Char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');

            //generating the config to be used in RCA Engine
            IEnumerable<ConcurrentEventsConfig> testGenerateConfig = RelatedEventsConfigs.generateConfig();

            List<ICommonSFItems> inputEvents = new List<ICommonSFItems>();

            if (load)
            {
                LoadPlugins();
                load = false;
            }
            foreach (var key in Plugins.Keys)
            {
                inputEvents = Plugins[key].ReturnEvents(inputEvents, startTimeUTC, endTimeUTC);
            }

            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);

            //******** This is the actual execution of the RCA
            //******** Returns a list of the events and its respective RCA for each event
            List<RCAEvents> simulEvents = new List<RCAEvents>();
            if (filteredInputEvents.Count > 0)
            {
                simulEvents = testRCA.GetSimultaneousEventsForEvent(testGenerateConfig, filteredInputEvents, inputEvents);
            }
            else
            {
                simulEvents = testRCA.GetSimultaneousEventsForEvent(testGenerateConfig, inputEvents, inputEvents);
            }
            return simulEvents;
        }

        public static List<ICommonSFItems> getFilteredInputEvents(string eventInstanceIds, string[] eventInstanceId, List<ICommonSFItems> inputEvents)
        {
            List<ICommonSFItems> filteredInputEvents = new List<ICommonSFItems>();
            if (!String.IsNullOrEmpty(eventInstanceIds))
            {
                foreach (var e in eventInstanceId)
                {
                    bool exists = false;
                    foreach (var inputEvent in inputEvents)
                    {
                        if (!String.IsNullOrEmpty(e) && inputEvent.EventInstanceId == e)
                        {
                            filteredInputEvents.Add(inputEvent);
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        Console.WriteLine($"EventInstanceId {e} does not exist \n");
                    }
                }
                if (filteredInputEvents.Count == 0)
                {
                    Environment.Exit(0);
                }
            }

            return filteredInputEvents;
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

