using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Tool-side registration request body sent to a platform's
    /// <c>registration_endpoint</c>. Mirrors OIDC client metadata
    /// (RFC 7591) plus the LTI <c>lti-tool-configuration</c> claim.
    /// </summary>
    public class ToolConfiguration
    {
        /// <summary>
        /// OIDC application_type. Required; must be <c>"web"</c> for LTI tools.
        /// Not defaulted, so a platform can tell when a tool leaves it out.
        /// </summary>
        [JsonPropertyName("application_type")]
        public string ApplicationType { get; set; }

        /// <summary>OAuth 2 response types — typically <c>["id_token"]</c>.</summary>
        [JsonPropertyName("response_types")]
        public IList<string> ResponseTypes { get; set; }

        /// <summary>OAuth 2 grant types — typically <c>["implicit", "client_credentials"]</c>.</summary>
        [JsonPropertyName("grant_types")]
        public IList<string> GrantTypes { get; set; }

        /// <summary>The OIDC initiate_login_uri.</summary>
        [JsonPropertyName("initiate_login_uri")]
        public string InitiateLoginUri { get; set; }

        /// <summary>Allowed launch redirect URIs.</summary>
        [JsonPropertyName("redirect_uris")]
        public IList<string> RedirectUris { get; set; }

        /// <summary>Human-readable client name.</summary>
        [JsonPropertyName("client_name")]
        public string ClientName { get; set; }

        /// <summary>The tool's homepage URL.</summary>
        [JsonPropertyName("client_uri")]
        public string ClientUri { get; set; }

        /// <summary>The tool's logo image URL.</summary>
        [JsonPropertyName("logo_uri")]
        public string LogoUri { get; set; }

        /// <summary>Space-separated list of scopes the tool will request.</summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Auth method at the token endpoint. Required; must be <c>"private_key_jwt"</c> for LTI tools.
        /// Not defaulted, so a platform can tell when a tool leaves it out.
        /// </summary>
        [JsonPropertyName("token_endpoint_auth_method")]
        public string TokenEndpointAuthMethod { get; set; }

        /// <summary>URL of the tool's public JWKS document.</summary>
        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        /// <summary>Echoed back by the platform on successful registration (Dynamic Registration §4.5).</summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>The LTI extension claim. Constant: <see cref="Constants.LtiClaims.LtiToolConfiguration"/>.</summary>
        [JsonPropertyName("https://purl.imsglobal.org/spec/lti-tool-configuration")]
        public LtiToolConfiguration LtiToolConfiguration { get; set; }
    }
}
