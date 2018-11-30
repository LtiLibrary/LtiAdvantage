using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class LineItem
    {
        /// <summary>
        /// The id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Optional, human-friendly label for this LineItem suitable for display. 
        /// For example, this label might be used as the heading of a column in a gradebook.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// The resource link id.
        /// </summary>
        [JsonProperty("ltiLinkId")]
        public string ResourceLinkId { get; set; }

        /// <summary>
        /// The non-link resource id.
        /// </summary>
        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        /// <summary>
        /// The maximum score allowed.
        /// </summary>
        [JsonProperty("scoreMaximum")]
        public double? ScoreMaximum { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
