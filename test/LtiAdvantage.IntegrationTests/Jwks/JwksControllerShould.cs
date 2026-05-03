using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.Jwks
{
    public class JwksControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public JwksControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(l => { l.AddConsole(); l.AddDebug(); })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ReturnPublicJwks_Anonymously()
        {
            var response = await _client.GetAsync(".well-known/jwks.json");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.StartsWith("application/jwk-set+json",
                response.Content.Headers.ContentType.ToString());

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("keys", out var keys));
            Assert.True(keys.GetArrayLength() >= 1);

            var first = keys[0];
            Assert.Equal("test-kid-1", first.GetProperty("kid").GetString());
            Assert.Equal("sig", first.GetProperty("use").GetString());
            Assert.Equal("RSA", first.GetProperty("kty").GetString());

            // Public key parameters MUST be present.
            Assert.False(string.IsNullOrEmpty(first.GetProperty("n").GetString()), "modulus n missing");
            Assert.False(string.IsNullOrEmpty(first.GetProperty("e").GetString()), "exponent e missing");
            Assert.Equal("RS256", first.GetProperty("alg").GetString());

            // Private RSA parameters MUST NOT be present.
            Assert.False(first.TryGetProperty("d",  out _), "private exponent d must not be published");
            Assert.False(first.TryGetProperty("p",  out _), "private prime p must not be published");
            Assert.False(first.TryGetProperty("q",  out _), "private prime q must not be published");
            Assert.False(first.TryGetProperty("dp", out _), "dp must not be published");
            Assert.False(first.TryGetProperty("dq", out _), "dq must not be published");
            Assert.False(first.TryGetProperty("qi", out _), "qi must not be published");
        }

        public void Dispose() { _client?.Dispose(); _server?.Dispose(); }
    }
}
