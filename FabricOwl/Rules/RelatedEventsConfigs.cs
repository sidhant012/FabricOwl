using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FabricOwl.Rules
{
    public class RelatedEventsConfigs
    {
        public static Dictionary<string, string> APEmap = new()
        {
            { "Deactivating application as application contents have changed.", "Stopping code package as application manifest contents have changes due to an app upgrade." },
            { "Deactivating since updating application version failed as part of upgrade.", "Stopping code package as upgrade failed." },
            { "Deactivating as Restart-DeployedCodePackage API invoked.", "Restarting code package as Restart-DeployedCodePackage API was invoked." },
            { "Deactivating becasue Fabric Node is closing.", "Stopping code package as SF node is shutting down."},
            { "Deactivating since no replica hosted.", "Stopping code package as process is idle and no replica/instance is hosted inside."},
            { "Deactivating since service package minor version change failed.", "Stopping code package as upgrade failed."},
            { "Deactivating since either RG changed, or upgrade is force restart, or major version changed.", "Stopping code package as RG changed or force restart was specified during application upgrade."},
            { "Deactivating as part of request from Activator CodePackage.", "Stopping code package on demand from activator code package."},
            { "Deactivating as part of upgrade since code package changed.", "Stopping code package as the code package contents have changed due to an app upgrade."},
        };

        const string forceKillPrefix = "Aborting since deactivation failed. ";
        //static readonly string owl = Path.GetFullPath(@"FabricOwl.exe");


        public static IEnumerable<ConcurrentEventsConfig> GenerateConfig()
        {
            string APE1 = GetResourceStream("FabricOwl.Rules.APE.json");

/*            if((Environment.ProcessPath).Equals(owl))
            {
                APE1 = File.ReadAllText(@"Rules\APE.json");
            } else
            {
                APE1 = File.ReadAllText(@"..\FabricOwl\Rules\APE.json");
            }*/

            //convert APE to IEnumerable<RelevantEventsConfig> type through Json DeserializeObject
            var APEConvert = JsonConvert.DeserializeObject<IEnumerable<RelevantEventsConfig>>(APE1);

            //Object.keys foreach loop with the converted APE (this is also where you will use the helper method)
            //the in foreach loop push/add (i think I will have to use concat) the return from the helper method to APE
            foreach (var key in APEmap.Keys)
            {
                var newConfig1 = GenerateConfigHelper(key, APEmap[key]);
                APEConvert = APEConvert.Concat(new[] { newConfig1 });
                var newConfig2 = GenerateConfigHelper(forceKillPrefix + key, APEmap[key], "but not graceful shutdown");
                APEConvert = APEConvert.Concat(new[] { newConfig2 });
            }

            string rulesConfig = GetResourceStream("FabricOwl.Rules.ExportedRules.json");
/*            if ((Environment.ProcessPath).Equals(owl))
            {
                rulesConfig = File.ReadAllText(@"Rules\ExportedRules.json");
            }
            else
            {
                rulesConfig = File.ReadAllText(@"..\FabricOwl\Rules\ExportedRules.json");
            }*/


            var rules = JsonConvert.DeserializeObject<IEnumerable<ConcurrentEventsConfig>>(rulesConfig);

            foreach(var r in rules)
            {
                if(r.EventType == "ApplicationProcessExited" || r.EventType == "ApplicationContainerInstanceExited")
                {
                    r.RelevantEventsType = APEConvert;
                }
            }
            return rules;
        }

        // To be revaluated for addition at some point
        public static IEnumerable<ConcurrentEventsConfig> AdditionalUserConfig()
        {
            return null;
        }

        private static RelevantEventsConfig GenerateConfigHelper(string text, string intendedDescription, string expectedPrefix = "")
        {
            string tempGenerate = GetResourceStream("FabricOwl.Rules.ConfigHelperAPE.json");
/*            if ((Environment.ProcessPath).Equals(owl))
            {
                tempGenerate = File.ReadAllText(@"Rules\ConfigHelperAPE.json");
            }
            else
            {
                tempGenerate = File.ReadAllText(@"..\FabricOwl\Rules\ConfigHelperAPE.json");
            }*/

            var generated = JsonConvert.DeserializeObject<RelevantEventsConfig>(tempGenerate);

            foreach(var prop in generated.PropertyMappings)
            {
                prop.TargetProperty = text;
            }
            generated.Result = "Expected Termination " + expectedPrefix + " - " + intendedDescription;

            return generated;
        }

        private static string GetResourceStream(string embeddedResource)
        {
            string result;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource))
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        } 
    }
}
