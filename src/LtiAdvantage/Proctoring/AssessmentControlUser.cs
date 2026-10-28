using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Identifies the candidate an Assessment Control Service request applies to.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-request.
    /// </summary>
    public class AssessmentControlUser
    {
        /// <summary>
        /// The issuer that identified the user, i.e. the platform's issuer identifier. Required.
        /// </summary>
        [JsonPropertyName("iss")]
        public string Iss { get; set; }

        /// <summary>
        /// The user's opaque identifier, matching the "sub" claim of the launch. Required.
        /// </summary>
        [JsonPropertyName("sub")]
        public string Sub { get; set; }
    }
}
