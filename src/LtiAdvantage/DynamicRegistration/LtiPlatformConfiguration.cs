using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Value of the LTI extension claim on a platform's OpenID configuration document.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#lti-platform-configuration
    /// </summary>
    public class LtiPlatformConfiguration
    {
        /// <summary>The product family code identifying the platform vendor.</summary>
        [JsonPropertyName("product_family_code")]
        public string ProductFamilyCode { get; set; }

        /// <summary>The platform's version string.</summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>The set of LTI message types this platform supports launching, plus optional placements.</summary>
        [JsonPropertyName("messages_supported")]
        public IList<MessageDescriptor> MessagesSupported { get; set; }

        /// <summary>Custom variable expansions the platform supports (e.g. <c>"Person.email.primary"</c>).</summary>
        [JsonPropertyName("variables")]
        public IList<string> Variables { get; set; }
    }
}
