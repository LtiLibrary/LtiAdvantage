using LtiAdvantage.Utilities;
using System;
using System.Text.Json.Serialization;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a score.
    /// </summary>
    public class Score
    {
        /// <summary>
        /// Status of the user toward activity's completion.
        /// </summary>
        [JsonPropertyName("activityProgress")]
        public ActivityProgress ActivityProgress { get; set; }

        /// <summary>
        /// A comment with the score.
        /// </summary>
        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Comment { get; set; }

        /// <summary>
        /// The status of the grading process.
        /// </summary>
        [JsonPropertyName("gradingProgress")]
        public GradingProgress GradingProgress { get; set; }

        /// <summary>
        /// The score. Optional: leave it null for a score that only reports progress, such as
        /// <see cref="AssignmentGradeServices.ActivityProgress.Started"/> with
        /// <see cref="AssignmentGradeServices.GradingProgress.NotReady"/>. When set,
        /// <see cref="ScoreMaximum"/> must be set too.
        /// </summary>
        [JsonPropertyName("scoreGiven")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ScoreGiven { get; set; }

        /// <summary>
        /// The maximum possible score. Required when <see cref="ScoreGiven"/> is set.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ScoreMaximum { get; set; }

        /// <summary>
        /// The UTC time the score was set. ISO 8601 format.
        /// </summary>
        [JsonPropertyName("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
