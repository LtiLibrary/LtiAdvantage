using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions implementing the tool side of LTI Dynamic Registration 1.0.
    /// </summary>
    public static class HttpClientDynamicRegistrationExtensions
    {
        // Leave unset properties out of the registration request instead of sending explicit nulls,
        // which strict platforms reject as invalid client metadata.
        private static readonly JsonSerializerOptions RequestJsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Fetches the platform's OpenID configuration document (<see cref="PlatformOpenIdConfiguration"/>)
        /// and validates it against the URL it was fetched from.
        /// </summary>
        /// <remarks>
        /// Per LTI Dynamic Registration 1.0 §3.4 and §3.5.1, the configuration URL must be the platform's
        /// <c>issuer</c> with a path appended, and must not contain a fragment. If it does not match, the
        /// registration must be aborted; this method throws <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <param name="client">The HTTP client.</param>
        /// <param name="openIdConfigurationUrl">The URL passed by the platform in the registration init launch.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <exception cref="HttpRequestException">The platform returned a non-success status code.</exception>
        /// <exception cref="InvalidOperationException">The response is empty or invalid, or the issuer does not match the URL.</exception>
        public static async Task<PlatformOpenIdConfiguration> GetPlatformOpenIdConfigurationAsync(
            this HttpClient client, string openIdConfigurationUrl, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(openIdConfigurationUrl))
                throw new ArgumentException("URL is required", nameof(openIdConfigurationUrl));

            const string operation = "Fetching the platform OpenID configuration";

            using var response = await client.GetAsync(openIdConfigurationUrl, cancellationToken).ConfigureAwait(false);
            var config = await ReadJsonOrThrowAsync<PlatformOpenIdConfiguration>(response, operation, cancellationToken)
                .ConfigureAwait(false);

            ValidateIssuer(config.Issuer, openIdConfigurationUrl);
            return config;
        }

        /// <summary>
        /// POSTs a tool registration request to the platform's <c>registration_endpoint</c>.
        /// Returns the platform-assigned <see cref="ToolConfiguration"/> (echoed config + <c>client_id</c>).
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="registrationEndpoint">The platform's registration endpoint URL.</param>
        /// <param name="registrationAccessToken">
        /// The <c>registration_token</c> from the registration init launch, sent as a Bearer token.
        /// It is optional in the spec: pass <c>null</c> or empty when the platform did not provide one,
        /// and the request is sent without an <c>Authorization</c> header.
        /// </param>
        /// <param name="tool">The tool configuration to register.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <exception cref="HttpRequestException">The platform returned a non-success status code.</exception>
        /// <exception cref="InvalidOperationException">The response is empty, invalid, or has no <c>client_id</c>.</exception>
        public static async Task<ToolConfiguration> RegisterToolAsync(
            this HttpClient client,
            string registrationEndpoint,
            string registrationAccessToken,
            ToolConfiguration tool,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(registrationEndpoint))
                throw new ArgumentException("URL is required", nameof(registrationEndpoint));
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            const string operation = "Dynamic Registration";

            var json = JsonSerializer.Serialize(tool, RequestJsonOptions);
            using var request = new HttpRequestMessage(HttpMethod.Post, registrationEndpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(registrationAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registrationAccessToken);
            }

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var registered = await ReadJsonOrThrowAsync<ToolConfiguration>(response, operation, cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(registered.ClientId))
                throw new InvalidOperationException($"{operation} failed: the platform response has no client_id.");

            return registered;
        }

        /// <summary>
        /// Throws if <paramref name="issuer"/> is not a valid issuer for <paramref name="openIdConfigurationUrl"/>.
        /// </summary>
        internal static void ValidateIssuer(string issuer, string openIdConfigurationUrl)
        {
            if (!Uri.TryCreate(openIdConfigurationUrl, UriKind.Absolute, out var configUri))
                throw new InvalidOperationException($"The OpenID configuration URL '{openIdConfigurationUrl}' is not an absolute URL.");
            if (!string.IsNullOrEmpty(configUri.Fragment))
                throw new InvalidOperationException("The OpenID configuration URL must not contain a fragment.");

            if (string.IsNullOrWhiteSpace(issuer) || !Uri.TryCreate(issuer, UriKind.Absolute, out var issuerUri))
                throw new InvalidOperationException("The platform OpenID configuration has no valid issuer.");
            if (!RegistrationUrl.IsAllowed(issuerUri))
                throw new InvalidOperationException($"The platform issuer '{issuer}' must use https.");
            if (!string.IsNullOrEmpty(issuerUri.Query) || !string.IsNullOrEmpty(issuerUri.Fragment))
                throw new InvalidOperationException($"The platform issuer '{issuer}' must not contain a query or fragment.");

            // The configuration URL must be the issuer with a path appended: same scheme, host and port,
            // and its path must start with the issuer's path on a segment boundary.
            var issuerPath = issuerUri.AbsolutePath.TrimEnd('/');
            var configPath = configUri.AbsolutePath;
            var sameAuthority = string.Equals(issuerUri.Scheme, configUri.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(issuerUri.Host, configUri.Host, StringComparison.OrdinalIgnoreCase)
                && issuerUri.Port == configUri.Port;
            var pathUnderIssuer = configPath.StartsWith(issuerPath, StringComparison.Ordinal)
                && (configPath.Length == issuerPath.Length || configPath[issuerPath.Length] == '/');

            if (!sameAuthority || !pathUnderIssuer)
                throw new InvalidOperationException(
                    $"The platform issuer '{issuer}' does not match the OpenID configuration URL '{openIdConfigurationUrl}'. " +
                    "The registration must be aborted.");
        }

        // Error bodies go into exception messages and from there into logs, so keep them short.
        private const int MaxErrorBodyLength = 500;

        private static string Truncate(string body) =>
            body.Length <= MaxErrorBodyLength ? body : body.Substring(0, MaxErrorBodyLength) + "… (truncated)";

        private static async Task<T> ReadJsonOrThrowAsync<T>(
            HttpResponseMessage response, string operation, CancellationToken cancellationToken) where T : class
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"{operation} failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {Truncate(body)}");
            }

            if (string.IsNullOrWhiteSpace(body))
                throw new InvalidOperationException($"{operation} failed: the platform returned an empty response.");

            T result;
            try
            {
                result = JsonSerializer.Deserialize<T>(body);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"{operation} failed: the platform response is not valid JSON.", ex);
            }

            return result ?? throw new InvalidOperationException($"{operation} failed: the platform returned an empty response.");
        }
    }
}
