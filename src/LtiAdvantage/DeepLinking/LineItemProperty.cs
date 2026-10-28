
using System.Text.Json.Serialization;
using LtiAdvantage.SubmissionReview;

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

        /// <summary>
        /// Optional. Whether the platform may release this line item's grades to learners.
        /// See https://www.imsglobal.org/spec/lti-dl/v2p0#lti-resource-link.
        /// </summary>
        [JsonPropertyName("gradesReleased")]
        public bool? GradesReleased { get; set; }

        /// <summary>
        /// Optional Submission Review 1.0 settings, so the line item created from this
        /// deep-linked item supports submission review from the start.
        /// See https://www.imsglobal.org/spec/lti-sr/v1p0.
        /// </summary>
        [JsonPropertyName("submissionReview")]
        public SubmissionReviewProperty SubmissionReview { get; set; }
    }
}
