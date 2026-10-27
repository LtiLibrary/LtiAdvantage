using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.Jwks
{
    public class JwksControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        public JwksControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
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

            // Private key operations are published as their public counterparts.
            var keyOps = first.GetProperty("key_ops").EnumerateArray().Select(o => o.GetString()).ToArray();
            Assert.Equal(new[] { "verify" }, keyOps);

            // The document may be cached briefly.
            Assert.Equal("public, max-age=300", response.Headers.CacheControl?.ToString());

            // Symmetric keys MUST NOT be published.
            Assert.Equal(1, keys.GetArrayLength());
        }

        public void Dispose() { _client?.Dispose(); _host?.Dispose(); }
    }
}
