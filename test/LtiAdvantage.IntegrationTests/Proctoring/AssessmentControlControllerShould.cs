using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.Lti;
using LtiAdvantage.Proctoring;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.Proctoring
{
    public class AssessmentControlControllerShould : IDisposable
    {
        private const string Url = "assessment-control";
        private readonly HttpClient _client;
        private readonly IHost _host;

        public AssessmentControlControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        [Fact]
        public async Task Reject_WhenScopeMissing()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Nrps.MembershipReadonly);

            var response = await PostAsync(FlagRequest());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ApplyAction_AndReturnResultingStatus()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);

            var request = FlagRequest();
            request.Action = AssessmentControlAction.Pause;

            var response = await PostAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = JsonSerializer.Deserialize<AssessmentControlResponse>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(AssessmentControlStatus.Paused, result.Status);
        }

        [Fact]
        public async Task RoundTripExtraTime()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);

            var request = FlagRequest();
            request.Action = AssessmentControlAction.Update;
            request.ExtraTime = 20;

            var response = await PostAsync(request);

            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AssessmentControlResponse>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(20, result.ExtraTime);
        }

        /// <summary>Proctoring 1.0 §5.1 required parameters are checked before the override runs.</summary>
        [Theory]
        [InlineData("user")]
        [InlineData("resource_link")]
        [InlineData("attempt_number")]
        [InlineData("incident_time")]
        [InlineData("incident_severity")]
        public async Task Reject_MissingRequiredFields(string field)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);
            var request = FlagRequest();
            switch (field)
            {
                case "user": request.User = null; break;
                case "resource_link": request.ResourceLink = null; break;
                case "attempt_number": request.AttemptNumber = null; break;
                case "incident_time": request.IncidentTime = null; break;
                case "incident_severity": request.IncidentSeverity = 1.5; break;
            }

            var response = await PostAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(field, await response.Content.ReadAsStringAsync());
        }

        /// <summary>Proctoring 1.0 §5: requests and responses use the ACS media type.</summary>
        [Fact]
        public async Task AcceptAndAnswerWithTheSpecMediaType()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);
            var json = JsonSerializer.Serialize(FlagRequest());

            var response = await _client.PostAsync(Url,
                new StringContent(json, Encoding.UTF8, "application/vnd.ims.lti-ap.v1.control+json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/vnd.ims.lti-ap.v1.control+json", response.Content.Headers.ContentType.MediaType);
        }

        /// <summary>
        /// Only an exact action name counts. Enum.TryParse alone would read "pause,resume" as
        /// Pause|Resume = Terminate, and numbers as whatever value they name.
        /// </summary>
        [Theory]
        [InlineData("extend")]
        [InlineData("pause,resume")]
        [InlineData("3")]
        [InlineData("99")]
        [InlineData(" pause")]
        public async Task Reject_AnUnrecognisedAction(string action)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);
            var json = JsonSerializer.Serialize(FlagRequest())
                .Replace("\"action\":\"flag\"", "\"action\":" + JsonSerializer.Serialize(action));
            Assert.Contains(JsonSerializer.Serialize(action), json);

            var response = await _client.PostAsync(Url, new StringContent(json, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task NotSendANullStatus()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ap.ControlAll);
            var request = FlagRequest();
            request.ReasonCode = "forget-status";

            var response = await PostAsync(request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("\"status\":null", body);
            // The test host runs as Production: no exception message in the response.
            Assert.DoesNotContain("must set the response Status", body);
        }

        private Task<HttpResponseMessage> PostAsync(AssessmentControlRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            return _client.PostAsync(Url, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private static AssessmentControlRequest FlagRequest() => new()
        {
            User = new AssessmentControlUser
            {
                Iss = "https://assessment.example.com",
                Sub = "2047534b3cc6d7086909"
            },
            ResourceLink = new ResourceLinkClaimValueType {Id = "398"},
            AttemptNumber = 1,
            Action = AssessmentControlAction.Flag,
            IncidentTime = new DateTime(2026, 7, 27, 10, 45, 33, DateTimeKind.Utc),
            IncidentSeverity = 0.1,
            ReasonCode = "12056",
            ReasonMsg = "Excessive background noise outside candidate control"
        };

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
