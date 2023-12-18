using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using LtiAdvantage.Lti;


namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// Represents a member in the Membership container of the Membership service.
    /// </summary>
    /// <remarks>
    /// Member must not contain more information about the person than would be included
    /// in a basic launch. For example, if PII is not sent in basic launch from the same
    /// context, it should not be included in member.
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-container-media-type.
    /// </remarks>
    public class Member
    {
        /// <summary>
        /// The primary email address for the person.
        /// Not specified if not included in a basic launch from same context.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The person's assigned family name.
        /// Not specified  if not included in a basic launch from same context. 
        /// </summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// The person's assigned first name.
        /// Not specified  if not included in a basic launch from same context. 
        /// </summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Contains context and resource link specific message parameters.
        /// </summary>
        /// <remarks>
        /// Included with <see cref="Member"/> when the membership list is filtered to
        /// users who can access a specific resource link.
        /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-container-media-type.
        /// </remarks>
        [JsonPropertyName("message")]
        public LtiResourceLinkRequest[] Message { get; set; }

        /// <summary>
        /// The person's assigned full name (typically their first name followed
        /// by their family name separated with a space).
        /// Not specified  if not included in a basic launch from same context. 
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A URL to an image for the person.
        /// Not specified  if not included in a basic launch from same context. 
        /// </summary>
        [JsonPropertyName("picture")]
        public Uri Picture { get; set; }

        /// <summary>
        /// The role/s this member has in the context. Does not include non-context roles.
        /// </summary>
        [JsonPropertyName("roles")]
        public Role[] Roles { get; set; }

        /// <summary>
        /// A unique identifier for the person as provisioned by an external system such as an SIS.
        /// </summary>
        [JsonPropertyName("lis_person_sourcedid")]
        public string LisPersonSourcedId { get; set; }

        /// <summary>
        /// Membership status is either Active or Inactive. Defaults to Active if not specified.
        /// </summary>
        /// <remarks>
        /// Use Deleted when reporting differences if membership no longer exists.
        /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-status.
        /// </remarks>
        [JsonPropertyName("status")]
        public MemberStatus? Status
        {
            get => _status;
            set => _status = value ?? MemberStatus.Active;
        }
        private MemberStatus? _status = MemberStatus.Active;

        /// <summary>
        /// A unique identifier for the person.
        /// Corresponds to the "sub" claim.
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// If the user id is changing with the migration to LTI 1.3, a platform should include the lti11_legacy_user_id
        /// as an additional member attribute. It should contain the userId value from LTI 1.1 Names and Roles Provisioning
        /// Service 1.0 for that same user.
        /// </summary>
        [JsonPropertyName("lti11_legacy_user_id")]
        public string Lti11LegacyUserId { get; set; }
    }
}
