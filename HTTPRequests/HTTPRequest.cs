using FabricOwl.IConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPRequests
{
    public class HTTPRequest : IPlugin
    {
        private static string apiVersion64 = "6.4";
        private static string apiVersion60 = "6.0";
        private static string clusterURL = "http://localhost:19080";
        public string GetApplicationsEventList(string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Applications-related events. The response is list of ApplicationEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getapplicationseventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Applications/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");
            return GetEvents(requestUri);
        }

        public string GetClusterEventList(string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Cluster-related events. The response is list of ClusterEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getclustereventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Cluster/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            return GetEvents(requestUri);
        }

        public string GetNodesEventList(string startTimeUTC, string endTimeUTC)
        {
            // Get Request to return all Nodes-related events. The response is list of NodesEvent objects.
            // https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getnodeseventlist
            var requestUri = new Uri($"{clusterURL}/EventsStore/Nodes/Events?api-version={apiVersion64}&StartTimeUtc={startTimeUTC}&EndTimeUtc={endTimeUTC}");

            return GetEvents(requestUri);
        }

        public string GetRepairTasksEventList()
        {
            // Get Request to return all RepairTasks events. The response is list of ReapirTasksEvent objects
            //https://learn.microsoft.com/en-us/rest/api/servicefabric/sfclient-api-getrepairtasklist
            var requestUri = new Uri($"{clusterURL}/$/GetRepairTaskList?api-version={apiVersion60}");

            return GetEvents(requestUri);
        }

        public static string GetEvents(Uri requestUri)
        {
            var request = WebRequest.Create(requestUri);
            request.Method = "GET";

            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            string data = reader.ReadToEnd();

            return data;
        }
    }
}
