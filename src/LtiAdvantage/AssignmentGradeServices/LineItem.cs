using System;
using System.Text.Json.Serialization;
using LtiAdvantage.SubmissionReview;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a gradebook column (e.g. an assignment).
    /// </summary>
    public class LineItem
    {
        /// <summary>
        /// The end date and time.
        /// </summary>
        [JsonPropertyName("endDateTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// The endpoint url for this item.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; set; }

        /// <summary>
        /// Optional, human-friendly label for this LineItem suitable for display. 
        /// For example, this label might be used as the heading of a column in a gradebook.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The resource link id.
        /// </summary>
        [JsonPropertyName("resourceLinkId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ResourceLinkId { get; set; }

        /// <summary>
        /// The non-link resource id.
        /// </summary>
        [JsonPropertyName("resourceId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ResourceId { get; set; }

        /// <summary>
        /// The maximum score allowed.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        public double? ScoreMaximum { get; set; }

        /// <summary>
        /// The start date and time.
        /// </summary>
        [JsonPropertyName("startDateTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Optional tag.
        /// </summary>
        [JsonPropertyName("tag")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Tag { get; set; }

        /// <summary>
        /// Optional. Whether the platform may release this line item's grades to learners.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0.
        /// </summary>
        [JsonPropertyName("gradesReleased")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? GradesReleased { get; set; }

        /// <summary>
        /// Submission Review extension. When present, the platform uses
        /// <c>SubmissionReviewExtension.Url</c> for LtiSubmissionReviewRequest
        /// launches against this line item.
        /// </summary>
        [JsonPropertyName("submissionReview")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SubmissionReviewProperty SubmissionReviewExtension { get; set; }
    }
}
