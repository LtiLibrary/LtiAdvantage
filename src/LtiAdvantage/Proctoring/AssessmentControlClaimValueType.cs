using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// LTI claim to include in an LtiStartProctoring message if the platform supports the
    /// Assessment Control Service.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#acs-claim.
    /// </summary>
    public class AssessmentControlClaimValueType
    {
        /// <summary>
        /// Fully resolved URL of the Assessment Control Service endpoint. Required.
        /// </summary>
        [JsonPropertyName("assessment_control_url")]
        public string AssessmentControlUrl { get; set; }

        /// <summary>
        /// The actions the platform permits the proctoring tool to request. Required.
        /// A tool must not request an action absent from this list.
        /// </summary>
        [JsonPropertyName("actions")]
        public AssessmentControlAction[] Actions { get; set; }
    }
}
