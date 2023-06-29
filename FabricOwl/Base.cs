using FabricOwl.IConfigs;
using FabricOwl.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace FabricOwl
{
    public class Base
    {

        /*
         * Still ToDo
         *  
         */
        protected static readonly Dictionary<string, IPlugin> Plugins = new();
        private const string PluginFolderName = "Plugins";
        private static string PluginPath;
        private static readonly string startTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow.AddDays(-7));
        private static readonly string endTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow);
        static bool load = true;
        // Another clusterURL to use once you have security credentials https://winlrc-sfrp-01.eastus.cloudapp.azure.com:19080

        public static async Task<List<RCAEvents>> GetRCA(string eventInstanceIds = "")
        {
            RCAEngine testRCA = new();

            eventInstanceIds = string.Concat(eventInstanceIds.Where(c => !char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');

            // Generating the config to be used in RCA Engine.
            IEnumerable<ConcurrentEventsConfig> testGenerateConfig = RelatedEventsConfigs.GenerateConfig();


            // generating the config to be used in RCA Engine based on user input TOTHINK: to add later
/*            if (!string.IsNullOrWhiteSpace(additionalConfig) && File.Exists(additionalConfig))
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
            }*/

            List<ICommonSFItems> inputEvents = new();

            if (load)
            {
                LoadPlugins();
                load = false;
            }

            foreach (var key in Plugins.Keys)
            {
                inputEvents.AddRange(await Plugins[key].ReturnEvents(startTimeUTC, endTimeUTC));
            }

            List<ICommonSFItems> filteredInputEvents = GetFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);

            //******** This is the actual execution of the RCA
            //******** Returns a list of the events and its respective RCA for each event
            List<RCAEvents> simulEvents = new();
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

        public static List<ICommonSFItems> GetFilteredInputEvents(string eventInstanceIds, string[] eventInstanceId, List<ICommonSFItems> inputEvents)
        {
            List<ICommonSFItems> filteredInputEvents = new();
            if (!string.IsNullOrWhiteSpace(eventInstanceIds))
            {
                foreach (var e in eventInstanceId)
                {
                    bool exists = false;
                    foreach (var inputEvent in inputEvents)
                    {
                        if (!string.IsNullOrWhiteSpace(e) && inputEvent.EventInstanceId == e)
                        {
                            filteredInputEvents.Add(inputEvent);
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        // Change this to Log message.
                        Console.WriteLine($"EventInstanceId {e} does not exist \n");
                    }
                }
            }

            return filteredInputEvents;
        }

        public static void LoadPlugins()
        {
            try
            {
                if (!Path.IsPathRooted(PluginFolderName))
                {
                    PluginPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), PluginFolderName);
                }

                foreach (var dll in Directory.EnumerateFiles(PluginPath, "*.dll"))
                {
                    AssemblyLoadContext assemblyLoadContext = new(dll);
                    Assembly assembly = assemblyLoadContext.LoadFromAssemblyPath(dll);
                    for (int i = 0; i < assembly.GetTypes().Length; i++)
                    {
                        try
                        {
                            var plugin = Activator.CreateInstance(assembly.GetTypes()[i]) as IPlugin;
                            if (plugin is not null)
                            {
                                Plugins.Add(Path.GetFileNameWithoutExtension(dll), plugin);
                                // Why continue with this inner loop at this point? Do you 
                                // expect to have multiple IPlugin impls in the same dll? You should break here, if not.
                            }
                        }
                        catch (MissingMethodException)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is DirectoryNotFoundException)
            {
                // TODO: Add log message here.
            }
        }
    }
}

