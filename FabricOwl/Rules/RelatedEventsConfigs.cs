using FabricOwl.IConfigs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FabricOwl.Rules
{
    public class RelatedEventsConfigs
    {
        public static Dictionary<string, string> APEmap = new Dictionary<string, string>{
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

        //this is the APE json
        static string APE1 = File.ReadAllText("C:\\Users\\sibhatia\\source\\repos\\FabricOwl\\FabricOwl\\Rules\\APE.json");

        public static IEnumerable<ConcurrentEventsConfig> generateConfig()
        {

            //convert APE to IEnumerable<RelevantEventsConfig> type through Json DeserializeObject
            var APEConvert = JsonConvert.DeserializeObject<IEnumerable<RelevantEventsConfig>>(APE1);

            //Object.keys foreach loop with the converted APE (this is also where you will use the helper method)
            //the in foreach loop push/add (i think I will have to use concat) the return from the helper method to APE
            foreach (var key in APEmap.Keys)
            {
                var newConfig1 = generateConfigHelper(key, APEmap[key]);
                APEConvert = APEConvert.Concat(new[] { newConfig1 });
                var newConfig2 = generateConfigHelper(forceKillPrefix + key, APEmap[key], "but not graceful shutdown");
                APEConvert = APEConvert.Concat(new[] { newConfig2 });
            }

            //then probably need to serialize to add to the json string below
            string convertedBackAPE = JsonConvert.SerializeObject(APEConvert, Formatting.Indented);

            //Console.WriteLine("Converted: " + convertedBackAPE);


            //this is RelatedEventsConfigs that you will insert the converted APE in its respective positions
/*            string ESC = File.ReadAllText("C:\\Users\\sibhatia\\source\\repos\\FabricOwl\\FabricOwl\\Rules\\ExportedRules.json");

            var temp = JsonConvert.DeserializeObject<IEnumerable<ConcurrentEventsConfig>>(ESC);

            Console.WriteLine("Converted: " + JsonConvert.SerializeObject(temp,Formatting.Indented));*/

            string exportedStringConfig = @"[
              {
                'eventType': 'ApplicationProcessExited',
                'relevantEventsType': " + convertedBackAPE + "," +
                "'result': 'ExitReason'," +
              "}," +
              "{" +
                "'eventType': 'ApplicationContainerInstanceExited'," +
                "'relevantEventsType': " + convertedBackAPE + "," +
                "'result': 'ExitReason'," +
              "}," +
              "{" +
                "'eventType': 'NodeDown'," +
                "'relevantEventsType': [" +
                  "{" +
                    "'eventType': 'NodeDeactivateStarted'," +
                    "'propertyMappings': [" +
                      "{" +
                        "'sourceProperty': 'NodeName'," +
                        "'targetProperty': 'NodeName'" +
                      "}," +
                      "{" +
                        "'sourceProperty': 'NodeInstance'," +
                        "'targetProperty': 'NodeInstance'" +
                      "}" +
                    "]," +
                  "}" +
                "]," +
                "'result': ''" +
              "}," +
              "{" +
                "'eventType': 'NodeDeactivateStarted'," +
                "'relevantEventsType': [" +
                  "{" +
                    "'eventType': 'RepairTask'," +
                    "'propertyMappings': [" +
                      "{" +
                        "'sourceProperty': 'BatchId'," +
                        "'targetProperty': 'TaskId'," +
                        "'sourceTransform': [" +
                          "{" +
                            "'type': 'trimFront'," +
                            "'value': '/'" +
                          "}" +
                        "]" +
                      "}" +
                    "]," +
                  "}" +
                "]," +
                "'result': ''" +
              "}," +
              "{" +
                "'eventType': 'RepairTask'," +
                "'relevantEventsType': [" +
                "]," +
                "'result': 'Action'" +
              "}," +
              "{" +
                "'eventType': 'PartitionReconfigurationStarted'," +
                "'relevantEventsType': [" +
                  "{" +
                    "'eventType': 'PartitionReconfigured'," +
                    "'propertyMappings': [" +
                      "{" +
                        "'sourceProperty': 'eventProperties.ActivityId'," +
                        "'targetProperty': 'eventProperties.ActivityId'" +
                      "}" +
                    "]," +
                  "}" +
                "]," +
                "'result': 'eventProperties.NewPrimaryNodeName'," +
                "'resultTransform': [" +
                  "{" +
                    "'type': 'prefix'," +
                    "'value': 'New Primary Node Name is '" +
                  "}" +
                "]" +
              "}," +
              "{" +
                "'eventType': 'PartitionReconfigured'," +
                "'relevantEventsType': [" +
                "]," +
                "'result': 'eventProperties.ReconfigType'" +
              "}," +
              "{" +
                "'eventType': 'ClusterNewHealthReport'," +
                "'relevantEventsType': [" +
                  "{" +
                    "'eventType': 'NodeDown'," +
                    "'propertyMappings': [" +
                      "{" +
                        "'sourceProperty': 'eventProperties.Description'," +
                        "'targetProperty': 'NodeName'," +
                        "'sourceTransform': [" +
                          "{" +
                            "'type': 'trimFront'," +
                            "'value': ':'" +
                          "}," +
                          "{" +
                            "'type': 'trimFront'," +
                            "'value': ':'" +
                          "}," +
                          "{" +
                            "'type': 'trimBack'," +
                            "'value': '('" +
                          "}," +
                          "{" +
                            "'type': 'trimWhiteSpace'" +
                          "}," +
                        "]" +
                      "}" +
                    "]," +
                  "}," +
                "]," +
                "'result': ''" +
              "}" +
            "]";

            //Then you will convert this to IEnumerable<ConcurrentEventsConfig> and return it
            var endConvertedConfig = JsonConvert.DeserializeObject<IEnumerable<ConcurrentEventsConfig>>(exportedStringConfig);

            //Console.WriteLine("Converted: " + JsonConvert.SerializeObject(endConvertedConfig,Formatting.Indented));

            return endConvertedConfig;
        }
        private static RelevantEventsConfig generateConfigHelper(string text, string intendedDescription, string expectedPrefix = "")
        {
            //this is generateConfig (the config that needs to be generated)
            string tempGenerate = @"{
                'eventType': 'self',
                'propertyMappings': [
                  {
                    'sourceProperty': 'ExitReason',
                    'targetProperty': '" + text + "'," +
                    "'sourceTransform': [{" +
                     "'type': 'trimFront'," +
                     "'value': '.'" +
                    "}," +
                    "{" +
                      "'type': 'trimBack'," +
                      "'value': 'For information'" +
                    "}," +
                    "{" +
                      "'type': 'trimBack'," +
                      "'value': 'ContainerName'" +
                    "}," +
                    "{" +
                      "'type': 'trimWhiteSpace'" +
                    "}]" +
                  "}" +
                "]," +
                "'result': 'Expected Termination " + expectedPrefix + " - " + intendedDescription + "'," +
              "}";

            //convert the tempGenerate to RelevantEventsConfig using JsonConvert.DeserializeObject<RelevantEventsConfig>(tempGenerate)
            var generate = JsonConvert.DeserializeObject<RelevantEventsConfig>(tempGenerate);

            //Console.WriteLine(generate.Result); *** REMOVE

            //return the converted generated config
            return generate;
        }
    }
}
