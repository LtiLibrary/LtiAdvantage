using System;
using System.Linq;
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
        /// The actions the platform permits the proctoring tool to request, exactly as sent.
        /// Required. A tool must not request an action absent from this list.
        /// </summary>
        /// <remarks>
        /// Kept as strings so that actions this library version does not know (a later spec
        /// revision may add some) survive a read and re-write unchanged. Use
        /// <see cref="Actions"/> for the recognised ones.
        /// </remarks>
        [JsonPropertyName("actions")]
        public string[] ActionNames { get; set; }

        /// <summary>
        /// The recognised actions in <see cref="ActionNames"/>; unrecognised names are left out.
        /// Setting this replaces <see cref="ActionNames"/>.
        /// </summary>
        [JsonIgnore]
        public AssessmentControlAction[] Actions
        {
            get => ActionNames?
                .Select(name => EnumNames.TryParse<AssessmentControlAction>(name, out var action) ? action : AssessmentControlAction.Unknown)
                .Where(action => action != AssessmentControlAction.Unknown)
                .ToArray();
            set => ActionNames = value?
                .Where(action => action != AssessmentControlAction.Unknown)
                .Select(action => action.ToString().ToLowerInvariant())
                .ToArray();
        }
    }
}
