using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.SubmissionReview
{
    /// <summary>
    /// The user whose submission is being reviewed.
    /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#for-user-claim
    /// </summary>
    public class ForUserClaimValueType
    {
        /// <summary>The user_id of the reviewed user.</summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>The reviewed user's full name.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>The reviewed user's given (first) name.</summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>The reviewed user's family (last) name.</summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>The reviewed user's email.</summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>The roles of the reviewed user in the context, as role URIs.</summary>
        /// <remarks>
        /// Strings, because a platform may send roles this library does not define. Use
        /// <see cref="Lti.RoleExtensions.TryParse"/> to read them as <see cref="Lti.Role"/> values and
        /// <see cref="Lti.RoleExtensions.ToUri"/> to write one.
        /// </remarks>
        [JsonPropertyName("roles")]
        public IList<string> Roles { get; set; }

        /// <summary>The person sourcedId of the reviewed user.</summary>
        [JsonPropertyName("person_sourcedid")]
        public string PersonSourcedId { get; set; }
    }
}
