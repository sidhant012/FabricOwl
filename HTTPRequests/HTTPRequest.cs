using FabricOwl.IConfigs;
using FabricOwl.SFObjects;
using Newtonsoft.Json;
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
        public async Task<List<ICommonSFItems>> GetApplicationsEventList(string startTimeUTC, string endTimeUTC)
        {
            List<ICommonSFItems> inputEvents = new();
            // Get Request to return all Applications-related events. The response is list of ApplicationEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getapplicationseventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Applications/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(await GetEvents(requestUri));
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
            var requestUri = new Uri($"{clusterURL}/EventsStore/Cluster/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var ClusterConvertEvents = JsonConvert.DeserializeObject<List<ClusterItem>>(await GetEvents(requestUri));
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
            var requestUri = new Uri($"{clusterURL}/EventsStore/Nodes/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(await GetEvents(requestUri));
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
            var requestUri = new Uri($"{clusterURL}/$/GetRepairTaskList?api-version={apiVersion60}");

            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(await GetEvents(requestUri));
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
            var requestUri = new Uri($"{clusterURL}/EventsStore/Partitions/Events?api-version={apiVersion72}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var PartitionConvertEvents = JsonConvert.DeserializeObject<List<PartitionItem>>(await GetEvents(requestUri));
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

        public static async Task<string> GetEvents(Uri requestUri)
        {
            string data = "";
            try
            {
                using HttpClient httpClient = new();
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                var request = await httpClient.GetAsync(requestUri);

                using var webResponse = request.Content;
                using var webStream = await webResponse.ReadAsStreamAsync();
                using var reader = new StreamReader(webStream);
                data = await reader.ReadToEndAsync();

                return data;
            } catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is TimeoutException)
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
