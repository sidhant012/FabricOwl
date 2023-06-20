﻿using FabricOwl.IConfigs;
using FabricOwl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FabricOwl
{
    public class Base
    {
        readonly HTTPRequest requests = new();

        private static readonly string startTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow.AddDays(-7));
        private static readonly string endTimeUTC = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow);
        //Another clusterURL to use once you have security credentials https://winlrc-sfrp-01.eastus.cloudapp.azure.com:19080

        public async Task<List<RCAEvents>> GetRCA(string eventInstanceIds = "")
        {
            RCAEngine testRCA = new();

            eventInstanceIds = String.Concat(eventInstanceIds.Where(c => !Char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');

            // generating the config to be used in RCA Engine
            IEnumerable<ConcurrentEventsConfig> testGenerateConfig = RelatedEventsConfigs.GenerateConfig();


            // generating the config to be used in RCA Engine based on user input TOTHINK: to add later
/*            if (!String.IsNullOrWhiteSpace(additionalConfig) && File.Exists(additionalConfig))
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

            List<ICommonSFItems> inputEvents = await requests.ReturnEvents(startTimeUTC, endTimeUTC);

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
            if (!String.IsNullOrWhiteSpace(eventInstanceIds))
            {
                foreach (var e in eventInstanceId)
                {
                    bool exists = false;
                    foreach (var inputEvent in inputEvents)
                    {
                        if (!String.IsNullOrWhiteSpace(e) && inputEvent.EventInstanceId == e)
                        {
                            filteredInputEvents.Add(inputEvent);
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        // Change this to Log message
                        Console.WriteLine($"EventInstanceId {e} does not exist \n");
                    }
                }
            }

            return filteredInputEvents;
        }
    }
}

