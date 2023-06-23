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
        public RCAEvents RelatedEvent { get; set; } //the next direct related event

        /// <summary>
        /// Reason for the current event.
        /// </summary>
        [JsonProperty("reasonForEvent")]
        public string ReasonForEvent { get; set; } // reason for the current event

        /// <summary>
        /// The event that is currently being looked at for analysis.
        /// </summary>
        [JsonProperty("inputEvent")]
        public ICommonSFItems InputEvent { get; set; } //the event that is currently being looked at for analysis
    }
}
