using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Represents the state of an assessment attempt as reported by the platform in an
    /// Assessment Control Service response.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-response.
    /// </summary>
    [JsonConverter(typeof(LowercaseEnumConverter<AssessmentControlStatus>))]
    public enum AssessmentControlStatus
    {
        /// <summary>
        /// The status was absent or is not recognised by this library version. Distinct from
        /// <see cref="None"/>, which the platform sends deliberately.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// There is no attempt in progress.
        /// </summary>
        None,

        /// <summary>
        /// The attempt is in progress.
        /// </summary>
        Running,

        /// <summary>
        /// The attempt is paused and can be resumed.
        /// </summary>
        Paused,

        /// <summary>
        /// The attempt was ended before the candidate finished.
        /// </summary>
        Terminated,

        /// <summary>
        /// The candidate finished the attempt.
        /// </summary>
        Complete
    }
}
