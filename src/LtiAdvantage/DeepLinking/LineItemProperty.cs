using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Line item definition.
    /// </summary>
    public class LineItemProperty
    {
        /// <summary>
        /// Optional label.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Optional resource id.
        /// </summary>
        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        /// <summary>
        /// Maximum score possible.
        /// </summary>
        [JsonProperty("scoreMaximum")]
        public float? ScoreMaximum { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
