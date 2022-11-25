using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.IConfigs
{
    public class ConcurrentEventsConfig
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; } // the event type we are investigating

        [JsonProperty("relevantEventsType")]
        public IEnumerable<RelevantEventsConfig> RelevantEventsType { get; set; } // possible causes we are considering

        [JsonProperty("result")]
        public string Result { get; set; } // resulting property we want to display for events (ex. Repair Jobs action)

        [JsonProperty("resultTransform")]
        public IEnumerable<Transform> ResultTransform { get; set; } // used to describe result transformations that we want to make
    }
}
