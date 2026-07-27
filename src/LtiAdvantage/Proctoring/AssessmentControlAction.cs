using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Represents the actions a proctoring tool may request of the platform through the
    /// Assessment Control Service.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-actions.
    /// </summary>
    [JsonConverter(typeof(AssessmentControlActionConverter))]
    public enum AssessmentControlAction
    {
        /// <summary>
        /// The action was absent or is not recognised by this library version.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Pause the assessment. The candidate cannot continue until it is resumed.
        /// </summary>
        Pause,

        /// <summary>
        /// Resume a paused assessment.
        /// </summary>
        Resume,

        /// <summary>
        /// End the assessment. The candidate cannot continue.
        /// </summary>
        Terminate,

        /// <summary>
        /// Update the assessment, for example to grant extra time.
        /// </summary>
        Update,

        /// <summary>
        /// Record an incident against the attempt without changing its state.
        /// </summary>
        Flag
    }
}
