using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Proctoring configuration exchanged between the platform and the proctoring tool.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#proctoring-settings-claim.
    /// </summary>
    public class ProctoringSettingsClaimValueType
    {
        /// <summary>
        /// Opaque, tool-specific settings. The platform stores and echoes this value without
        /// interpreting it; only the proctoring tool understands its format.
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
