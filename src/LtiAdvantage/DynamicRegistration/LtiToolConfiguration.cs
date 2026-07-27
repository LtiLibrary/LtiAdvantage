using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// LTI extension claim on a Dynamic Registration request body.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#lti-tool-configuration
    /// </summary>
    public class LtiToolConfiguration
    {
        /// <summary>The tool's primary domain (without scheme, no path).</summary>
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>Additional domains the tool also serves from.</summary>
        [JsonPropertyName("secondary_domains")]
        public IList<string> SecondaryDomains { get; set; }

        /// <summary>Optional deployment id requested by the tool.</summary>
        [JsonPropertyName("deployment_id")]
        public string DeploymentId { get; set; }

        /// <summary>Default target_link_uri for launches.</summary>
        [JsonPropertyName("target_link_uri")]
        public string TargetLinkUri { get; set; }

        /// <summary>Custom parameters to merge into every launch.</summary>
        [JsonPropertyName("custom_parameters")]
        public IDictionary<string, string> CustomParameters { get; set; }

        /// <summary>Human-readable description of the tool.</summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>The OpenID claims this tool requests (e.g. <c>"name"</c>, <c>"email"</c>).</summary>
        [JsonPropertyName("claims")]
        public IList<string> Claims { get; set; }

        /// <summary>The LTI message types this tool supports receiving.</summary>
        [JsonPropertyName("messages")]
        public IList<MessageDescriptor> Messages { get; set; }
    }
}
