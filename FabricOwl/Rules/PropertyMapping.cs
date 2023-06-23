using Newtonsoft.Json;
using System.Collections.Generic;

namespace FabricOwl.Rules
{
    public class PropertyMapping
    {
        [JsonProperty("sourceProperty")]
        public string SourceProperty { get; set; }

        [JsonProperty("targetProperty")]
        public string TargetProperty { get; set; }

        /// <summary>
        /// Used to describe source transformations that we want to make.
        /// </summary>
        [JsonProperty("sourceTransform")]
        public IEnumerable<Transform> SourceTransform { get; set; }

        /// <summary>
        /// Used to describe target transformations that we want to make
        /// </summary>
        [JsonProperty("targetTransform")]
        public IEnumerable<Transform> TargetTransform { get; set; }
    }
}
