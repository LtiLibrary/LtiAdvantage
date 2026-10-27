using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using LtiAdvantage.IdentityModel.Client;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class HttpClientDynamicRegistrationExtensionsShould
    {
        [Fact]
        public async Task FetchPlatformConfiguration_FromOpenidConfigurationUrl()
        {
            var configJson = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var handler = new StubHandler((req, ct) =>
            {
                Assert.Equal("https://platform.example.com/.well-known/openid-configuration",
                    req.RequestUri.ToString());
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(configJson, Encoding.UTF8, "application/json")
                };
            });
            using var client = new HttpClient(handler);

            var config = await client.GetPlatformOpenIdConfigurationAsync(
                "https://platform.example.com/.well-known/openid-configuration");

            Assert.Equal("https://platform.example.com", config.Issuer);
            Assert.Equal("https://platform.example.com/lti/register", config.RegistrationEndpoint);
        }

        [Fact]
        public async Task PostRegistration_WithBearerToken()
        {
            var responseJson = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            // Pretend the platform echoed back with a client_id assigned.
            var responseTool = JsonSerializer.Deserialize<ToolConfiguration>(responseJson);
            responseTool.ClientId = "client-9";
            var responseBody = JsonSerializer.Serialize(responseTool);

            HttpRequestMessage capturedRequest = null;
            var handler = new StubHandler((req, ct) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
                };
            });
            using var client = new HttpClient(handler);

            var request = JsonSerializer.Deserialize<ToolConfiguration>(
                TestUtils.LoadReferenceJsonFile("ToolConfiguration"));

            var result = await client.RegisterToolAsync(
                "https://platform.example.com/lti/register",
                "registration-token-xyz",
                request);

            Assert.Equal("client-9", result.ClientId);
            Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
            Assert.Equal("registration-token-xyz", capturedRequest.Headers.Authorization.Parameter);
            Assert.Equal("application/json", capturedRequest.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task SurfaceErrorBody_OnRegistrationFailure()
        {
            const string errorBody = """{"error":"invalid_redirect_uri","error_description":"bad uri"}""";
            var handler = new StubHandler((req, ct) =>
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(errorBody, Encoding.UTF8, "application/json")
                });
            using var client = new HttpClient(handler);

            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.RegisterToolAsync(
                    "https://platform.example.com/lti/register",
                    "tok",
                    new ToolConfiguration()));

            Assert.Contains("400", ex.Message);
            Assert.Contains("invalid_redirect_uri", ex.Message);
        }

        [Fact]
        public async Task PostRegistration_WithoutAuthorizationHeader_WhenNoTokenGiven()
        {
            HttpRequestMessage capturedRequest = null;
            using var client = new HttpClient(new StubHandler((req, ct) =>
            {
                capturedRequest = req;
                return JsonResponse(HttpStatusCode.Created, """{"client_id":"client-9"}""");
            }));

            var result = await client.RegisterToolAsync(
                "https://platform.example.com/lti/register", null, new ToolConfiguration());

            Assert.Equal("client-9", result.ClientId);
            Assert.Null(capturedRequest.Headers.Authorization);
        }

        [Fact]
        public async Task PostRegistration_WithoutNullProperties()
        {
            string requestBody = null;
            using var client = new HttpClient(new StubHandler((req, ct) =>
            {
                requestBody = req.Content.ReadAsStringAsync(ct).Result;
                return JsonResponse(HttpStatusCode.Created, """{"client_id":"client-9"}""");
            }));

            await client.RegisterToolAsync(
                "https://platform.example.com/lti/register", "tok",
                new ToolConfiguration { ClientName = "Example Tool" });

            using var doc = JsonDocument.Parse(requestBody);
            Assert.Equal("Example Tool", doc.RootElement.GetProperty("client_name").GetString());
            Assert.False(doc.RootElement.TryGetProperty("client_id", out _), "unset client_id must not be sent");
            Assert.False(doc.RootElement.TryGetProperty("logo_uri", out _), "unset logo_uri must not be sent");
        }

        [Theory]
        [InlineData("""{"client_name":"Example Tool"}""")]
        [InlineData("")]
        [InlineData("null")]
        [InlineData("not json")]
        public async Task ThrowOnRegistrationResponse_WithoutClientId(string responseBody)
        {
            using var client = new HttpClient(new StubHandler((req, ct) =>
                JsonResponse(HttpStatusCode.Created, responseBody)));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                client.RegisterToolAsync("https://platform.example.com/lti/register", "tok", new ToolConfiguration()));
        }

        [Theory]
        [InlineData("https://platform.example.com", "https://platform.example.com/.well-known/openid-configuration")]
        [InlineData("https://platform.example.com/", "https://platform.example.com/lti/config?reg=1")]
        [InlineData("https://platform.example.com/tenant1", "https://platform.example.com/tenant1/.well-known/openid-configuration")]
        public async Task AcceptPlatformConfiguration_WhenUrlIsUnderIssuer(string issuer, string url)
        {
            using var client = new HttpClient(new StubHandler((req, ct) =>
                JsonResponse(HttpStatusCode.OK, $$"""{"issuer":"{{issuer}}"}""")));

            var config = await client.GetPlatformOpenIdConfigurationAsync(url);

            Assert.Equal(issuer, config.Issuer);
        }

        [Theory]
        // Another host claiming to be the real platform.
        [InlineData("https://real-lms.example", "https://attacker.example/.well-known/openid-configuration")]
        // A subdomain that does not match the issuer.
        [InlineData("https://platform.example.com", "https://evil.platform.example.com/.well-known/openid-configuration")]
        // Same host, but outside the issuer's path.
        [InlineData("https://platform.example.com/tenant1", "https://platform.example.com/tenant1evil/.well-known/openid-configuration")]
        // Issuer must use https.
        [InlineData("http://platform.example.com", "http://platform.example.com/.well-known/openid-configuration")]
        // URL must not contain a fragment.
        [InlineData("https://platform.example.com", "https://platform.example.com/.well-known/openid-configuration#x")]
        // Missing issuer.
        [InlineData(null, "https://platform.example.com/.well-known/openid-configuration")]
        public async Task RejectPlatformConfiguration_WhenIssuerDoesNotMatchUrl(string issuer, string url)
        {
            var body = issuer == null ? "{}" : $$"""{"issuer":"{{issuer}}"}""";
            using var client = new HttpClient(new StubHandler((req, ct) => JsonResponse(HttpStatusCode.OK, body)));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                client.GetPlatformOpenIdConfigurationAsync(url));
        }

        [Fact]
        public async Task NameTheOperation_WhenFetchingConfigurationFails()
        {
            using var client = new HttpClient(new StubHandler((req, ct) =>
                JsonResponse(HttpStatusCode.NotFound, "")));

            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.GetPlatformOpenIdConfigurationAsync("https://platform.example.com/.well-known/openid-configuration"));

            Assert.Contains("OpenID configuration", ex.Message);
            Assert.Contains("404", ex.Message);
        }

        private static HttpResponseMessage JsonResponse(HttpStatusCode status, string body) =>
            new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _fn;
            public StubHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> fn) => _fn = fn;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken c)
                => Task.FromResult(_fn(r, c));
        }
    }
}
