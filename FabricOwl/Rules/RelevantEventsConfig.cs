using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.Rules
{
    public class RelevantEventsConfig
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("propertyMappings")]
        public IEnumerable<PropertyMapping> PropertyMappings { get; set; }

        [JsonProperty("selfTransform")]
        public IEnumerable<Transform> SelfTransform { get; set; } //used to describe self transformations that we want to make to strings

        [JsonProperty("result")]
        public string Result { get; set; }
    }
}
