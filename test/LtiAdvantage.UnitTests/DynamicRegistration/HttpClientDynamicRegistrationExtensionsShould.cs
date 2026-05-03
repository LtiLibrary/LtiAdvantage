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

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _fn;
            public StubHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> fn) => _fn = fn;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken c)
                => Task.FromResult(_fn(r, c));
        }
    }
}
