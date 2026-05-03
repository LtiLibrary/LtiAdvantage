using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        /// <summary>
        /// Fetches the platform's OpenID configuration document (<see cref="PlatformOpenIdConfiguration"/>).
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="openIdConfigurationUrl">The URL passed by the platform in the registration init launch.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<PlatformOpenIdConfiguration> GetPlatformOpenIdConfigurationAsync(
            this HttpClient client, string openIdConfigurationUrl, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(openIdConfigurationUrl))
                throw new ArgumentException("URL is required", nameof(openIdConfigurationUrl));

            using var response = await client.GetAsync(openIdConfigurationUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new HttpRequestException(
                    $"Dynamic Registration failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {errorBody}");
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(body);
        }

        /// <summary>
        /// POSTs a tool registration request to the platform's <c>registration_endpoint</c>
        /// using the bearer registration token supplied during the registration init launch.
        /// Returns the platform-assigned <see cref="ToolConfiguration"/> (echoed config + <c>client_id</c>).
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="registrationEndpoint">The platform's registration endpoint URL.</param>
        /// <param name="registrationAccessToken">Bearer token from the registration init launch.</param>
        /// <param name="tool">The tool configuration to register.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
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
            if (string.IsNullOrWhiteSpace(registrationAccessToken))
                throw new ArgumentException("Token is required", nameof(registrationAccessToken));
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            var json = JsonSerializer.Serialize(tool);
            using var request = new HttpRequestMessage(HttpMethod.Post, registrationEndpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registrationAccessToken);

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new HttpRequestException(
                    $"Dynamic Registration failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {errorBody}");
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<ToolConfiguration>(body);
        }
    }
}
