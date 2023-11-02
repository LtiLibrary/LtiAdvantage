
using System.Text.Json.Serialization;

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
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Optional resource id.
        /// </summary>
        [JsonPropertyName("resourceId")]
        public string ResourceId { get; set; }

        /// <summary>
        /// Maximum score possible.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        public float? ScoreMaximum { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
    }
}
