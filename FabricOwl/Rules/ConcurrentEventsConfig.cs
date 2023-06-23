using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace FabricOwl.Rules
{
    public class ConcurrentEventsConfig
    {
        /// <summary>
        /// The event type we are investigating.
        /// </summary>
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Possible causes we are considering.
        /// </summary>
        [JsonProperty("relevantEventsType")]
        public IEnumerable<RelevantEventsConfig> RelevantEventsType { get; set; }

        /// <summary>
        /// Resulting property we want to display for events (ex. Repair Jobs action).
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        /// <summary>
        ///  Used to describe result transformations that we want to make.
        /// </summary>
        [JsonProperty("resultTransform")]
        public IEnumerable<Transform> ResultTransform { get; set; }
    }
}
