using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

        [Fact]
        public async Task NotWriteNulls()
        {
            var body = await _client.GetStringAsync(".well-known/openid-configuration");

            Assert.Contains("messages_supported", body);
            Assert.DoesNotContain("null", body);
        }

        /// <summary>
        /// An app whose OpenID Connect server already serves .well-known/openid-configuration can
        /// move the LTI document; the named route, and so Url.Link, follow it.
        /// </summary>
        [Fact]
        public async Task ServeTheDocumentAtAMovedRoute()
        {
            using var host = TestHostFactory.Create(services => services.Configure<MvcOptions>(options =>
                options.MapLtiRoute(Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService,
                    "lti/.well-known/openid-configuration")));
            using var client = host.GetTestClient();

            var moved = await client.GetAsync("lti/.well-known/openid-configuration");
            var old = await client.GetAsync(".well-known/openid-configuration");

            Assert.Equal(HttpStatusCode.OK, moved.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, old.StatusCode);
        }

        [Fact]
        public async Task MoveTheJwksRouteToo()
        {
            using var host = TestHostFactory.Create(services => services.Configure<MvcOptions>(options =>
                options.MapLtiRoute(Constants.ServiceEndpoints.Jwks.JwksService, "keys/jwks.json")));
            using var client = host.GetTestClient();

            var configuration = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(
                await client.GetStringAsync(".well-known/openid-configuration"));

            Assert.EndsWith("/keys/jwks.json", configuration.JwksUri);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(new Uri(configuration.JwksUri).PathAndQuery)).StatusCode);
        }

        /// <summary>A mistyped route name stops the app at startup instead of leaving the route where it was.</summary>
        [Fact]
        public void RefuseToMoveARouteThatDoesNotExist()
        {
            var ex = Assert.ThrowsAny<InvalidOperationException>(() => TestHostFactory.Create(services =>
                services.Configure<MvcOptions>(options => options.MapLtiRoute("no-such-route", "anywhere"))));

            Assert.Contains("no-such-route", ex.Message);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
