using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.IConfigs
{
    public class RCAItem : EventPropertiesCollection
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("eventInstanceId")]
        public string EventInstanceId { get; set; }

        [JsonProperty("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}
