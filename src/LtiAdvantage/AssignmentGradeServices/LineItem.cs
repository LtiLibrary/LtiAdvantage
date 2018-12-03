using System;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class LineItem
    {
        /// <summary>
        /// The end date and time.
        /// </summary>
        [JsonProperty("end_date_time")]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// The endpoint url for this item.
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
        [JsonProperty("lti_link_id")]
        public string ResourceLinkId { get; set; }

        /// <summary>
        /// The non-link resource id.
        /// </summary>
        [JsonProperty("resource_id")]
        public string ResourceId { get; set; }

        /// <summary>
        /// The maximum score allowed.
        /// </summary>
        [JsonProperty("score_maximum")]
        public double? ScoreMaximum { get; set; }

        /// <summary>
        /// The start date and time.
        /// </summary>
        [JsonProperty("start_date_time")]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
