using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Describes a supported LTI message type. Used in both
    /// <see cref="LtiPlatformConfiguration"/> (what the platform launches)
    /// and <c>LtiToolConfiguration</c> (what the tool can receive).
    /// </summary>
    public class MessageDescriptor
    {
        /// <summary>The message type, e.g. <c>"LtiResourceLinkRequest"</c>.</summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>Per-message target_link_uri (tool side).</summary>
        [JsonPropertyName("target_link_uri")]
        public string TargetLinkUri { get; set; }

        /// <summary>Per-message label shown by the platform (tool side).</summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>Per-message icon URI (tool side).</summary>
        [JsonPropertyName("icon_uri")]
        public string IconUri { get; set; }

        /// <summary>Custom parameters to merge into this launch (tool side).</summary>
        [JsonPropertyName("custom_parameters")]
        public IDictionary<string, string> CustomParameters { get; set; }

        /// <summary>Placements this message is offered for (e.g. <c>"ContentArea"</c>, <c>"RichTextEditor"</c>).</summary>
        [JsonPropertyName("placements")]
        public IList<string> Placements { get; set; }

        /// <summary>Roles required for this launch (tool side).</summary>
        [JsonPropertyName("roles")]
        public IList<string> Roles { get; set; }
    }
}
