using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class ScoresControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string ScoreUrl = "/context/1234/lineitems/1234/scores/1234";
        private const string ScoresUrl = "/context/1234/lineitems/1234/scores";

        public ScoresControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }
        
        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.Score, HttpStatusCode.OK, Constants.MediaTypes.Score)]
        [InlineData(Constants.LtiScopes.Ags.ScoreReadonly, HttpStatusCode.Forbidden, "")]
        public async Task AddScore_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
#if NET7_0_OR_GREATER
            var scoreContent = new StringContent(JsonSerializer.Serialize(new Score()),
                Encoding.UTF8, new MediaTypeHeaderValue(Constants.MediaTypes.Score));
#else
            var scoreContent = new StringContent(JsonSerializer.Serialize(new Score()),
                Encoding.UTF8, Constants.MediaTypes.Score);
#endif
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.PostAsync(ScoresUrl, scoreContent);
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
        public async Task ReturnScore_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
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
            _host?.Dispose();
        }
    }
}
