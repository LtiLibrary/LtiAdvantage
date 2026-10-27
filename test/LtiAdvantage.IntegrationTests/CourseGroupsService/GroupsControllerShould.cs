using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.CourseGroupsService;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.CourseGroupsService
{
    public class GroupsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string GroupsUrl = "context/1234/groups";

        public GroupsControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Gs.ContextGroupReadonly, HttpStatusCode.OK,
            Constants.MediaTypes.ContextGroupContainer)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.Forbidden, "")]
        public async Task ReturnGroups_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(GroupsUrl);
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        /// <summary>
        /// The user filter is spelled user_id in the specification, which is not the default
        /// binding name for a userId parameter.
        /// </summary>
        [Fact]
        public async Task BindTheUserIdQueryParameter()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Gs.ContextGroupReadonly);

            var response = await _client.GetAsync($"{GroupsUrl}?user_id=user-7&limit=5");

            response.EnsureSuccessStatusCode();
            var container = JsonSerializer.Deserialize<ContextGroupContainer>(
                await response.Content.ReadAsStringAsync());

            Assert.Equal("user-7", container.UserId);
            Assert.Equal("context/1234/groups", container.Id);
            Assert.Contains("limit=5", JsonSerializer.Serialize(container.Groups));
        }

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
