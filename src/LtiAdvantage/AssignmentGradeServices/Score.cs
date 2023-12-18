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
        public string Comment { get; set; }

        /// <summary>
        /// The status of the grading process.
        /// </summary>
        [JsonPropertyName("gradingProgress")]
        public GradingProgess GradingProgress { get; set; }

        /// <summary>
        /// The score.
        /// </summary>
        [JsonPropertyName("scoreGiven")]
        public double ScoreGiven { get; set; }

        /// <summary>
        /// The maximum possible score.
        /// </summary>
        [JsonPropertyName("scoreMaximum")]
        public double ScoreMaximum { get; set; }

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
