using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// Represents the results returned by the Membership service.
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-container-media-type.
    /// </summary>
    public class MembershipContainer
    {
        /// <summary>
        /// The ID of these results. Typically the request URL to Membership service.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Context information for the list of members.
        /// </summary>
        [JsonPropertyName("context")]
        public Context Context { get; set; }

        /// <summary>
        /// The list of members in the specified context.
        /// </summary>
        [JsonPropertyName("members")]
        public ICollection<Member> Members { get; set; }
    }
}
