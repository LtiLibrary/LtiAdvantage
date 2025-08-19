using System.Text.Json.Serialization;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// LTI claim to include in the LtiResourceLinkRequest if the platform
    /// supports the Names and Role Service.
    /// </summary>
    public class NamesRoleServiceClaimValueType
    {
        /// <summary>
        /// </summary>
        public NamesRoleServiceClaimValueType()
        {
            ServiceVersions = new[] {Version};
        }

        /// <summary>
        /// The version of this implementation.
        /// </summary>
        public const string Version = "2.0";

        /// <summary>
        /// Fully resolved URL to service.
        /// </summary>
        [JsonPropertyName("context_memberships_url")]
        public string ContextMembershipUrl { get; set; }

        /// <summary>
        /// The scope to request to access this service.
        /// It's optional and not provided from every platform.
        /// If not provided, the default scope is "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly".
        /// </summary>
        [JsonPropertyName("scope")]
        public string[]? Scope { get; set; }

        /// <summary>
        /// Service version. Default is <see cref="Version"/>.
        /// </summary>
        [JsonPropertyName("service_versions")]
        public string[] ServiceVersions { get; set; }
    }
}
