using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// A platform's OpenID Provider Metadata document (RFC 8414) including
    /// the LTI Dynamic Registration extension claim
    /// (<c>https://purl.imsglobal.org/spec/lti-platform-configuration</c>).
    /// </summary>
    public class PlatformOpenIdConfiguration
    {
        /// <summary>The platform's issuer identifier URL.</summary>
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        /// <summary>OAuth 2 authorization endpoint URL.</summary>
        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        /// <summary>OAuth 2 token endpoint URL.</summary>
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        /// <summary>Auth methods supported at the token endpoint (typically <c>private_key_jwt</c>).</summary>
        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public IList<string> TokenEndpointAuthMethodsSupported { get; set; }

        /// <summary>JWS algorithms supported for client assertions.</summary>
        [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
        public IList<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; }

        /// <summary>URL of the platform's public JWKS document.</summary>
        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        /// <summary>URL the tool POSTs its registration request to.</summary>
        [JsonPropertyName("registration_endpoint")]
        public string RegistrationEndpoint { get; set; }

        /// <summary>OAuth 2 scopes the platform supports.</summary>
        [JsonPropertyName("scopes_supported")]
        public IList<string> ScopesSupported { get; set; }

        /// <summary>OAuth 2 response types the platform supports (typically <c>id_token</c>).</summary>
        [JsonPropertyName("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; }

        /// <summary>Subject types supported (typically <c>public</c>).</summary>
        [JsonPropertyName("subject_types_supported")]
        public IList<string> SubjectTypesSupported { get; set; }

        /// <summary>JWS signing algorithms supported for ID tokens (typically <c>RS256</c>).</summary>
        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public IList<string> IdTokenSigningAlgValuesSupported { get; set; }

        /// <summary>Standard claims the platform supports.</summary>
        [JsonPropertyName("claims_supported")]
        public IList<string> ClaimsSupported { get; set; }

        /// <summary>The LTI extension claim. Constant: <see cref="Constants.LtiClaims.LtiPlatformConfiguration"/>.</summary>
        [JsonPropertyName("https://purl.imsglobal.org/spec/lti-platform-configuration")]
        public LtiPlatformConfiguration LtiPlatformConfiguration { get; set; }
    }
}
