using Newtonsoft.Json;

namespace FabricOwl.Rules
{
    public class Transform
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
