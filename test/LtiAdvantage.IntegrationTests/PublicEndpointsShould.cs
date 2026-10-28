using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace LtiAdvantage.IntegrationTests
{
    /// <summary>
    /// Platforms commonly require authentication everywhere through a fallback policy. The
    /// metadata endpoints must stay reachable anyway: tools fetch them before holding credentials.
    /// </summary>
    public class PublicEndpointsShould : IDisposable
    {
        private readonly IHost _host;
        private readonly HttpClient _client;

        public PublicEndpointsShould()
        {
            _host = TestHostFactory.Create(services => services.AddAuthorization(options =>
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            _client = _host.GetTestClient();
        }

        [Theory]
        [InlineData(".well-known/jwks.json")]
        [InlineData(".well-known/openid-configuration")]
        public async Task StayAnonymous_UnderARequireAuthenticationFallbackPolicy(string url)
        {
            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _host.Dispose();
        }
    }
}
