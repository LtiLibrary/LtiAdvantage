using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.OpenIdConnect
{
    public class OpenIdConfigurationControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        public OpenIdConfigurationControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// A tool has to be able to read this before it holds any credential, so the endpoint is
        /// anonymous like the JWKS endpoint it points at.
        /// </summary>
        [Fact]
        public async Task ReturnConfiguration_Anonymously()
        {
            var response = await _client.GetAsync(".well-known/openid-configuration");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.StartsWith("application/json", response.Content.Headers.ContentType.ToString());

            var configuration = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(
                await response.Content.ReadAsStringAsync());

            Assert.Equal("https://platform.example.com", configuration.Issuer);
            Assert.Equal(new[] {"id_token"}, configuration.ResponseTypesSupported);
            Assert.Equal(new[] {"private_key_jwt"}, configuration.TokenEndpointAuthMethodsSupported);
            Assert.Equal("ExamplePlatform", configuration.LtiPlatformConfiguration.ProductFamilyCode);
        }

        /// <summary>
        /// The published URLs must be absolute and must actually resolve, otherwise dynamic
        /// registration sends the tool somewhere that does not exist.
        /// </summary>
        [Fact]
        public async Task PublishAbsoluteUrlsThatResolve()
        {
            var response = await _client.GetAsync(".well-known/openid-configuration");
            var configuration = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(
                await response.Content.ReadAsStringAsync());

            Assert.True(Uri.IsWellFormedUriString(configuration.JwksUri, UriKind.Absolute),
                $"jwks_uri '{configuration.JwksUri}' is not an absolute URL");
            Assert.True(Uri.IsWellFormedUriString(configuration.RegistrationEndpoint, UriKind.Absolute),
                $"registration_endpoint '{configuration.RegistrationEndpoint}' is not an absolute URL");

            var jwks = await _client.GetAsync(configuration.JwksUri);
            Assert.Equal(HttpStatusCode.OK, jwks.StatusCode);
        }

        /// <summary>
        /// The advertised scopes are what a tool asks for during registration, so the new Tier 2
        /// services have to be discoverable there.
        /// </summary>
        [Fact]
        public async Task AdvertiseTheSupportedScopes()
        {
            var response = await _client.GetAsync(".well-known/openid-configuration");
            var configuration = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(
                await response.Content.ReadAsStringAsync());

            Assert.Contains(Constants.LtiScopes.Gs.ContextGroupReadonly, configuration.ScopesSupported);
            Assert.Contains(Constants.LtiScopes.Ap.ControlAll, configuration.ScopesSupported);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
