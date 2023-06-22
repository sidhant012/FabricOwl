using Newtonsoft.Json;
using System.Collections.Generic;

namespace FabricOwl.Rules
{
    public class RelevantEventsConfig
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("propertyMappings")]
        public IEnumerable<PropertyMapping> PropertyMappings { get; set; }

        /// <summary>
        /// Used to describe self transformations that we want to make to strings.
        /// </summary>
        [JsonProperty("selfTransform")]
        public IEnumerable<Transform> SelfTransform { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }
    }
}
