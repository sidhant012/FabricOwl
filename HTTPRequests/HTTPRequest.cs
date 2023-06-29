using FabricOwl.IConfigs;
using FabricOwl.SFObjects;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HTTPRequests
{
    public class HTTPRequest : IPlugin
    {
        private readonly string apiVersion64 = "6.4";
        private readonly string apiVersion60 = "6.0";
        private readonly string apiVersion72 = "7.2";
        private readonly string clusterURL = "http://localhost:19080";

        // Don't catch the exceptions that you want Polly to retry.
        private readonly AsyncRetryPolicy retryPolicy =
            Policy.Handle<HttpRequestException>()
                  .Or<TimeoutException>()
                  .WaitAndRetryAsync(
                    new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                    });

        public async Task<List<ICommonSFItems>> GetApplicationsEventList(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();

            // Get Request to return all Applications-related events. The response is list of ApplicationEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getapplicationseventlist
            Uri requestUri = new($"{clusterURL}/EventsStore/Applications/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");
            string eventList = await retryPolicy.ExecuteAsync(() => GetEvents(requestUri));
            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(eventList);
            if (ApplicationConvertEvents == null || ApplicationConvertEvents.Count == 0)
            {
                return inputEvents;
            }
            else
            {
                inputEvents.AddRange(ApplicationConvertEvents);
            }
            return inputEvents;
        }

        public async Task<List<ICommonSFItems>> GetClusterEventList(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();

            // Get Request to return all Cluster-related events. The response is list of ClusterEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getclustereventlist
            Uri requestUri = new($"{clusterURL}/EventsStore/Cluster/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");
            string eventList = await retryPolicy.ExecuteAsync(() => GetEvents(requestUri));
            var ClusterConvertEvents = JsonConvert.DeserializeObject<List<ClusterItem>>(eventList);
            if (ClusterConvertEvents == null || ClusterConvertEvents.Count == 0)
            {
                return inputEvents;
            }
            else
            {
                inputEvents.AddRange(ClusterConvertEvents);
            }
            return inputEvents;
        }

        public async Task<List<ICommonSFItems>> GetNodesEventList(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();

            // Get Request to return all Nodes-related events. The response is list of NodesEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getnodeseventlist
            Uri requestUri = new($"{clusterURL}/EventsStore/Nodes/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");
            string eventList = await retryPolicy.ExecuteAsync(() => GetEvents(requestUri));
            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(eventList);
            if (NodeConvertEvents == null || NodeConvertEvents.Count == 0)
            {
                return inputEvents;
            }
            else
            {
                inputEvents.AddRange(NodeConvertEvents);
            }
            return inputEvents;
        }

        public async Task<List<ICommonSFItems>> GetRepairTasksEventList()
        {
            List<ICommonSFItems> inputEvents = new();

            // Get Request to return all RepairTasks events. The response is list of ReapirTasksEvent objects
            //https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getrepairtasklist
            Uri requestUri = new($"{clusterURL}/$/GetRepairTaskList?api-version={apiVersion60}");
            string eventList = await retryPolicy.ExecuteAsync(() => GetEvents(requestUri, 5));
            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(eventList);
            if(RepairConvertEvents == null || RepairConvertEvents.Count == 0)
            {
                return inputEvents;
            } 
            else
            {
                SetRepairValues(RepairConvertEvents);
                inputEvents.AddRange(RepairConvertEvents);
            }

            return inputEvents;
        }

        public async Task<List<ICommonSFItems>> GetPartitionsEventList(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();

            //Get Request to return all the Partition events. The response is list of ParitionEvent objects
            //https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getpartitionseventlist
            Uri requestUri = new($"{clusterURL}/EventsStore/Partitions/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");
            string eventList = await retryPolicy.ExecuteAsync(() => GetEvents(requestUri));
            var PartitionConvertEvents = JsonConvert.DeserializeObject<List<PartitionItem>>(eventList);
            if (PartitionConvertEvents == null || PartitionConvertEvents.Count == 0)
            {
                return inputEvents;
            }
            else
            {
                inputEvents.AddRange(PartitionConvertEvents);
            }
            return inputEvents;
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

        public static async Task<string> GetEvents(Uri requestUri, int timeoutSeconds = 10)
        {
            string data = string.Empty;
            try
            {
                using HttpClient httpClient = new();
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                using var request = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
                using var webResponse = request.Content;
                using var webStream = await webResponse.ReadAsStreamAsync();
                using var reader = new StreamReader(webStream);
                data = await reader.ReadToEndAsync();

                return data;
            } 
            catch (Exception e) when (e is ArgumentException or TaskCanceledException)
            {
                
            }
            return data;
        }

        public async Task<List<ICommonSFItems>> ReturnEvents(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();

            inputEvents.AddRange(await GetNodesEventList(startTimeUTC, endTimeUTC));
            inputEvents.AddRange(await GetApplicationsEventList(startTimeUTC, endTimeUTC));
            inputEvents.AddRange(await GetRepairTasksEventList());
            inputEvents.AddRange(await GetClusterEventList(startTimeUTC, endTimeUTC));
            inputEvents.AddRange(await GetPartitionsEventList(startTimeUTC, endTimeUTC));

            return inputEvents;
        }
    }
}
