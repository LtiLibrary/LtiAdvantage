using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.CourseGroupsService
{
    public class GroupSetsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string GroupSetsUrl = "context/1234/groups/sets";

        public GroupSetsControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// Check the return status code and content-type. Group sets share the groups scope.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Gs.ContextGroupReadonly, HttpStatusCode.OK,
            Constants.MediaTypes.ContextGroupSetContainer)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.Forbidden, "")]
        public async Task ReturnGroupSets_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(GroupSetsUrl);
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
