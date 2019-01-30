using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.NamesRoleProvisioningService
{
    public class MembershipControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        private const string MembershipUrl = "context/1234/membership";

        public MembershipControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.OK, Constants.MediaTypes.MembershipContainer)]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.Forbidden, "")]
        public async void ReturnMembership_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
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
            _server?.Dispose();
        }
    }
}
