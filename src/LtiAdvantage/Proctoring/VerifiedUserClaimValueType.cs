using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// The user identity attributes the proctoring tool verified before allowing the
    /// assessment to start. Sent by the tool on the LtiStartAssessment message.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#verified-user-claim.
    /// </summary>
    /// <remarks>
    /// Only the attributes the tool actually verified are populated. An attribute being
    /// absent means "not verified", not "did not match".
    /// </remarks>
    public class VerifiedUserClaimValueType
    {
        /// <summary>The verified given (first) name.</summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>The verified family (last) name.</summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>The verified full name.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>The verified email address.</summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>A URL to an image of the verified user, for example the identity photo taken at check-in.</summary>
        [JsonPropertyName("picture")]
        public string Picture { get; set; }
    }
}
