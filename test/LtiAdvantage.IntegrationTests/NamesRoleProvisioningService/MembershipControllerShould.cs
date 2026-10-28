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

        /// <summary>
        /// The Course Groups extension must not change membership responses for callers that did
        /// not ask for groups: no "group_enrollments": null on every member.
        /// </summary>
        [Fact]
        public async Task NotWriteNullGroupEnrollments_WhenGroupsWereNotRequested()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl);

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain("group_enrollments", await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Group enrollments are Course Groups data: a tool holding only the NRPS scope must not
        /// get them by adding ?groups=true.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, false)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly + " " + Constants.LtiScopes.Gs.ContextGroupReadonly, true)]
        public async Task IncludeGroups_OnlyWithTheGroupsScope(string scopes, bool expectGroups)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scopes);

            var response = await _client.GetAsync(MembershipUrl + "?groups=true");

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectGroups, body.Contains("group_enrollments"));
        }

        /// <summary>
        /// Even an override that loads group enrollments regardless must not leak them to a tool
        /// without the groups scope: the base class strips them.
        /// </summary>
        [Fact]
        public async Task StripGroupEnrollments_WithoutTheGroupsScope_EvenIfTheOverrideAddsThem()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl + "?groups=true&rlid=always-groups");

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain("group_enrollments", await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Withholding groups from one caller must not change the objects the override shares:
        /// a later caller with the groups scope still sees the enrollments.
        /// </summary>
        [Fact]
        public async Task NotChangeSharedMembers_WhenWithholdingGroups()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);
            var withheld = await _client.GetAsync(MembershipUrl + "?groups=true&rlid=shared-member");

            Assert.DoesNotContain("group_enrollments", await withheld.Content.ReadAsStringAsync());
            Assert.NotNull(Controllers.MembershipController.SharedMember.GroupEnrollments);
        }

        [Fact]
        public async Task StripGroupEnrollments_FromAJsonResultToo()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl + "?groups=true&rlid=json-result");

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("shared-user", body);
            Assert.DoesNotContain("group_enrollments", body, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// NRPS shows the full role URI in the role filter; the simple context role name and the
        /// enum name work too.
        /// </summary>
        [Theory]
        [InlineData("http%3A%2F%2Fpurl.imsglobal.org%2Fvocab%2Flis%2Fv2%2Fmembership%23Learner", "ContextLearner")]
        [InlineData("http://purl.imsglobal.org/vocab/lis/v2/membership%23Instructor", "ContextInstructor")]
        [InlineData("Learner", "ContextLearner")]
        [InlineData("ContextLearner", "ContextLearner")]
        [InlineData("http%3A%2F%2Fpurl.imsglobal.org%2Fvocab%2Flis%2Fv2%2Finstitution%2Fperson%23Learner", "InstitutionLearner")]
        public async Task BindTheRoleFilter(string role, string expected)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl + "?role=" + role);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, string.Join(",", response.Headers.GetValues("x-role")));
        }

        [Theory]
        [InlineData("NotARole")]
        [InlineData("3")]
        public async Task ReturnBadRequest_ForAnUnknownRole(string role)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl + "?role=" + role);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>The override gets the request's abort token, so it can stop work for a caller that left.</summary>
        [Fact]
        public async Task PassTheRequestAbortedToken_ToTheOverride()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await _client.GetAsync(MembershipUrl + "?rlid=check-cancellation");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
