using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.Rules
{
    public class PropertyMapping
    {
        [JsonProperty("sourceProperty")]
        public string SourceProperty { get; set; }

        [JsonProperty("targetProperty")]
        public string TargetProperty { get; set; }

        [JsonProperty("sourceTransform")]
        public IEnumerable<Transform> SourceTransform { get; set; } //used to describe source transformations that we want to make

        [JsonProperty("targetTransform")]
        public IEnumerable<Transform> TargetTransform { get; set; } //used to describe target transformations that we want to make
    }
}
