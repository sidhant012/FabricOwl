using FabricOwl.IConfigs;
using FabricOwl.SFObjects;
using Newtonsoft.Json;
using System.Net;

namespace HTTPRequests
{
    public class HTTPRequest : IPlugin
    {
        private string apiVersion64 = "6.4";
        private string apiVersion60 = "6.0";
        private string clusterURL = "http://localhost:19080";
        public List<ICommonSFItems> GetApplicationsEventList(List<ICommonSFItems> inputEvents, string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Applications-related events. The response is list of ApplicationEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getapplicationseventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Applications/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var ApplicationConvertEvents = JsonConvert.DeserializeObject<List<ApplicationItem>>(GetEvents(requestUri));
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

        public List<ICommonSFItems> GetClusterEventList(List<ICommonSFItems> inputEvents, string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Cluster-related events. The response is list of ClusterEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getclustereventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Cluster/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var ClusterConvertEvents = JsonConvert.DeserializeObject<List<ClusterItem>>(GetEvents(requestUri));
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

        public List<ICommonSFItems> GetNodesEventList(List<ICommonSFItems> inputEvents, string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Nodes-related events. The response is list of NodesEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getnodeseventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Nodes/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            var NodeConvertEvents = JsonConvert.DeserializeObject<List<NodeItem>>(GetEvents(requestUri));
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

        public List<ICommonSFItems> GetRepairTasksEventList(List<ICommonSFItems> inputEvents)
        {
            // Get Request to return all RepairTasks events. The response is list of ReapirTasksEvent objects
            //https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getrepairtasklist
            var requestUri = new Uri($"{clusterURL}/$/GetRepairTaskList?api-version={apiVersion60}");

            var RepairConvertEvents = JsonConvert.DeserializeObject<List<RepairItem>>(GetEvents(requestUri));
            if(RepairConvertEvents == null || RepairConvertEvents.Count == 0)
            {
                return inputEvents;
            } else
            {
                SetRepairValues(RepairConvertEvents);
                inputEvents.AddRange(RepairConvertEvents);
            }

            return inputEvents;
        }

        public List<RepairItem> SetRepairValues(List<RepairItem> list)
        {
            foreach (var l in list)
            {
                l.EventInstanceId = l.TaskId;
                l.TimeStamp = l.History.CreatedUtcTimestamp;
            }
            return list;
        }

        public string GetEvents(Uri requestUri)
        {
            string data = "";
            try
            {
                var request = WebRequest.Create(requestUri);
                request.Timeout = 700;
                request.Method = "GET";

                using var webResponse = request.GetResponse();
                using var webStream = webResponse.GetResponseStream();

                using var reader = new StreamReader(webStream);
                data = reader.ReadToEnd();

                return data;
            } catch (WebException e) 
            {
            }

            return data;
        }

        public List<ICommonSFItems> ReturnEvents(List<ICommonSFItems> inputEvents, string startTimeUTC, string endTimeUTC)
        {
            inputEvents = GetNodesEventList(inputEvents, startTimeUTC, endTimeUTC);
            inputEvents = GetApplicationsEventList(inputEvents, startTimeUTC, endTimeUTC);
            inputEvents = GetRepairTasksEventList(inputEvents);
            inputEvents = GetClusterEventList(inputEvents, startTimeUTC, endTimeUTC);
            return inputEvents;
        }
    }
}
