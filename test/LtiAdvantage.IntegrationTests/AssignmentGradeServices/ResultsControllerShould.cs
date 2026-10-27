using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class ResultsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string ResultsUrl = "context/1234/lineitems/1234/results";

        public ResultsControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.ResultReadonly, HttpStatusCode.OK, Constants.MediaTypes.ResultContainer)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden, "")]
        public async Task ReturnResult_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
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
            _host?.Dispose();
        }
    }
}
