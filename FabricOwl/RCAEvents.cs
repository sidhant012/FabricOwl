using FabricOwl.IConfigs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl
{
    //This is the format in which the RCA will be returned
    public class RCAEvents : RCAItem
    {
        [JsonProperty("relatedEvent")]
        public RCAEvents RelatedEvent { get; set; } //the next direct related event

        [JsonProperty("reasonForEvent")]
        public string ReasonForEvent { get; set; } // reason for the current event

        [JsonProperty("inputEvent")]
        public ICommonSFItems InputEvent { get; set; } //the event that is currently being looked at for analysis
    }
}
