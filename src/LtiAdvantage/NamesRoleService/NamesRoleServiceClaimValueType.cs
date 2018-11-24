using Newtonsoft.Json;

namespace LtiAdvantage.NamesRoleService
{
    /// <summary>
    /// LTI claim to include in the LtiResourceLinkRequest if the platform
    /// supports the Names and Role Service.
    /// </summary>
    public class NamesRoleServiceClaimValueType
    {
        /// <summary>
        /// Create an instance of the <see cref="NamesRoleServiceClaimValueType"/> with
        /// the default value for <see cref="ServiceVersion"/>.
        /// </summary>
        public NamesRoleServiceClaimValueType()
        {
            ServiceVersion = "2.0";
        }

        /// <summary>
        /// Fully resolved URL to service.
        /// </summary>
        [JsonProperty("context_memberships_url")]
        public string ContextMembershipUrl { get; set; }

        /// <summary>
        /// Service version. Defaults to "2.0".
        /// </summary>
        [JsonProperty("service_version")]
        public string ServiceVersion { get; set; }
    }
}
