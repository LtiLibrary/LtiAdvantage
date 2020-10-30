using System;
using System.Net;
using System.Net.Http;
using System.Text;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class ScoresControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        private const string ScoreUrl = "/context/1234/lineitems/1234/scores/1234";
        private const string ScoresUrl = "/context/1234/lineitems/1234/scores";

        public ScoresControllerShould()
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
        [InlineData(Constants.LtiScopes.Ags.Score, HttpStatusCode.OK, Constants.MediaTypes.Score)]
        [InlineData(Constants.LtiScopes.Ags.ScoreReadonly, HttpStatusCode.Forbidden, "")]
        public async void AddScore_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            var scoreContent = new StringContent(JsonConvert.SerializeObject(new Score()),
                Encoding.UTF8, Constants.MediaTypes.Score);

            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.PostAsync(ScoresUrl, scoreContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.Score, HttpStatusCode.OK, Constants.MediaTypes.Score)]
        [InlineData(Constants.LtiScopes.Ags.ScoreReadonly, HttpStatusCode.OK, Constants.MediaTypes.Score)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.Forbidden, "")]
        public async void ReturnScore_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(ScoreUrl);
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
