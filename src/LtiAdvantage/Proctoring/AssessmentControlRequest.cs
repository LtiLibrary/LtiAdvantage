using System;
using System.Text.Json.Serialization;
using LtiAdvantage.Lti;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// A control action the proctoring tool asks the platform to take against an
    /// assessment attempt in progress.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-request.
    /// </summary>
    public class AssessmentControlRequest
    {
        /// <summary>
        /// The candidate the action applies to. Required.
        /// </summary>
        [JsonPropertyName("user")]
        public AssessmentControlUser User { get; set; }

        /// <summary>
        /// The resource link of the assessment. Only <see cref="ResourceLinkClaimValueType.Id"/>
        /// is required.
        /// </summary>
        [JsonPropertyName("resource_link")]
        public ResourceLinkClaimValueType ResourceLink { get; set; }

        /// <summary>
        /// The attempt the action applies to. Required.
        /// </summary>
        /// <remarks>
        /// Read leniently: the specification's examples show the attempt number both as a
        /// JSON number and as a quoted string.
        /// </remarks>
        [JsonPropertyName("attempt_number")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? AttemptNumber { get; set; }

        /// <summary>
        /// The action to take. Required, and must be one of the actions the platform
        /// advertised in <see cref="AssessmentControlClaimValueType.Actions"/>.
        /// </summary>
        [JsonPropertyName("action")]
        public AssessmentControlAction Action { get; set; }

        /// <summary>
        /// When the incident prompting this action occurred. Required.
        /// </summary>
        [JsonPropertyName("incident_time")]
        public DateTime? IncidentTime { get; set; }

        /// <summary>
        /// How serious the incident is, from 0 (least) to 1 (most).
        /// </summary>
        /// <remarks>
        /// Read leniently: the specification's example shows this as a quoted string.
        /// </remarks>
        [JsonPropertyName("incident_severity")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double? IncidentSeverity { get; set; }

        /// <summary>
        /// Additional minutes to grant the candidate. Only meaningful with
        /// <see cref="AssessmentControlAction.Update"/>.
        /// </summary>
        [JsonPropertyName("extra_time")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? ExtraTime { get; set; }

        /// <summary>
        /// A tool-specific code categorising the reason for the action.
        /// </summary>
        [JsonPropertyName("reason_code")]
        public string ReasonCode { get; set; }

        /// <summary>
        /// A human-readable explanation of the reason for the action.
        /// </summary>
        [JsonPropertyName("reason_msg")]
        public string ReasonMsg { get; set; }
    }
}
