using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class ResultsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        private const string ResultsUrl = "context/1234/lineitems/1234/results";

        public ResultsControllerShould()
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
        [InlineData(Constants.LtiScopes.Ags.ResultReadonly, HttpStatusCode.OK, Constants.MediaTypes.ResultContainer)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden, "")]
        public async void ReturnResult_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(ResultsUrl);
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
