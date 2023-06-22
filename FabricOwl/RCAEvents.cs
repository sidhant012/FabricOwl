using FabricOwl.IConfigs;
using Newtonsoft.Json;

namespace FabricOwl
{
    // This is the format in which the RCA will be returned.
    public class RCAEvents : RCAItem
    {
        /// <summary>
        /// The next direct related event.
        /// </summary>
        [JsonProperty("relatedEvent")]
        public RCAEvents RelatedEvent { get; set; }

        /// <summary>
        /// Reason for the current event.
        /// </summary>
        [JsonProperty("reasonForEvent")]
        public string ReasonForEvent { get; set; }

        /// <summary>
        /// The event that is currently being looked at for analysis.
        /// </summary>
        [JsonProperty("inputEvent")]
        public ICommonSFItems InputEvent { get; set; }
    }
}
