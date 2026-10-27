using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.SubmissionReview
{
    /// <summary>
    /// Submission Review extension on an AGS LineItem.
    /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#submissionreview-extension
    /// </summary>
    public class SubmissionReviewProperty
    {
        /// <summary>
        /// The submission statuses for which a review is possible, e.g. <c>"Submitted"</c>,
        /// <c>"Completed"</c>, <c>"Graded"</c>, or <c>"Any"</c>. If absent, the platform
        /// treats it as reviewable for any status.
        /// </summary>
        [JsonPropertyName("reviewableStatus")]
        public IList<string> ReviewableStatus { get; set; }

        /// <summary>
        /// Optional label the platform shows for the review link.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The launch URL the platform should use when launching back into
        /// the tool to review a submission for this line item.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional custom parameters to include with each Submission Review launch.
        /// </summary>
        [JsonPropertyName("custom")]
        public IDictionary<string, string> Custom { get; set; }
    }
}
