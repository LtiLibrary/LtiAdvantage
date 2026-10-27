using System.Text.Json.Serialization;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Error body returned when a registration request is rejected
    /// (RFC 7591 §3.2.2 format, used by LTI Dynamic Registration 1.0 §3.6.2).
    /// </summary>
    public class RegistrationError
    {
        /// <summary>Error code, e.g. <c>invalid_client_metadata</c> or <c>invalid_redirect_uri</c>.</summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>Human-readable description of the problem.</summary>
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>Creates an <c>invalid_client_metadata</c> error with the given description.</summary>
        public static RegistrationError InvalidClientMetadata(string description) =>
            new RegistrationError { Error = "invalid_client_metadata", ErrorDescription = description };
    }
}
