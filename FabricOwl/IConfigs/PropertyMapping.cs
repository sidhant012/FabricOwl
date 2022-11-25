using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl.IConfigs
{
    public class PropertyMapping
    {
        [JsonProperty("sourceProperty")]
        public object SourceProperty { get; set; }

        [JsonProperty("targetProperty")]
        public object TargetProperty { get; set; }

        [JsonProperty("sourceTransform")]
        public IEnumerable<Transform> SourceTransform { get; set; } //used to describe source transformations that we want to make

        [JsonProperty("targetTransform")]
        public IEnumerable<Transform> TargetTransform  { get; set; } //used to describe target transformations that we want to make
    }
}
