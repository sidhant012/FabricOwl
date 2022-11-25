using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.IConfigs
{
    public class EventPropertiesCollection
    {
        [JsonProperty("eventProperties")]
        public Dictionary<string, object> EventProperties { get; set; }
    }
}
