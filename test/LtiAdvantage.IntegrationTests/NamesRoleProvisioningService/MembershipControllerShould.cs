using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.NamesRoleProvisioningService
{
    public class MembershipControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string MembershipUrl = "context/1234/membership";

        public MembershipControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.OK, Constants.MediaTypes.MembershipContainer)]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.Forbidden, "")]
        public async Task ReturnMembership_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(MembershipUrl);
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
