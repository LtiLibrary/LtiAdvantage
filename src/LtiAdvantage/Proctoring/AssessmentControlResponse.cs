using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// The platform's report of the assessment attempt after applying a control action.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-response.
    /// </summary>
    public class AssessmentControlResponse
    {
        /// <summary>
        /// The state of the attempt after the action was applied.
        /// </summary>
        [JsonPropertyName("status")]
        public AssessmentControlStatus Status { get; set; }

        /// <summary>
        /// The total extra minutes granted to the candidate for this attempt, counting any
        /// previously granted time — not just the minutes added by this request.
        /// </summary>
        [JsonPropertyName("extra_time")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? ExtraTime { get; set; }
    }
}
