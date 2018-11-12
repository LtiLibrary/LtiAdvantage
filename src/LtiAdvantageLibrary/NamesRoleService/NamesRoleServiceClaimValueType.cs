using Newtonsoft.Json;

namespace LtiAdvantageLibrary.NamesRoleService
{
    public class NamesRoleServiceClaimValueType
    {
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
