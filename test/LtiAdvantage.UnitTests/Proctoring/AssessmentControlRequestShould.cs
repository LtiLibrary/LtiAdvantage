using System;
using System.Text.Json;
using LtiAdvantage.Proctoring;
using Xunit;

namespace LtiAdvantage.UnitTests.Proctoring
{
    public class AssessmentControlRequestShould
    {
        /// <summary>
        /// Deserialize the example assessment control request from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-request.
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("AssessmentControlRequest");
            var request = JsonSerializer.Deserialize<AssessmentControlRequest>(referenceJson);

            Assert.Equal("https://assessment.org", request.User.Iss);
            Assert.Equal("2047534b3cc6d7086909", request.User.Sub);
            Assert.Equal("398", request.ResourceLink.Id);
            Assert.Equal(1, request.AttemptNumber);
            Assert.Equal(AssessmentControlAction.Flag, request.Action);
            Assert.Equal(new DateTime(2018, 2, 1, 10, 45, 33, DateTimeKind.Utc),
                request.IncidentTime.Value.ToUniversalTime());
            Assert.Equal("12056", request.ReasonCode);
            Assert.Equal("Excessive background noise outside candidate control", request.ReasonMsg);
        }

        /// <summary>
        /// The specification's example quotes incident_severity, so the numeric property has to
        /// accept a string as well as a number.
        /// </summary>
        [Theory]
        [InlineData("\"0.1\"")]
        [InlineData("0.1")]
        public void ReadIncidentSeverityFromNumberOrString(string severity)
        {
            var json = $"{{\"action\":\"flag\",\"incident_severity\":{severity}}}";

            var request = JsonSerializer.Deserialize<AssessmentControlRequest>(json);

            Assert.Equal(0.1, request.IncidentSeverity);
        }

        [Fact]
        public void WriteActionAsLowercaseString()
        {
            var request = new AssessmentControlRequest {Action = AssessmentControlAction.Terminate};

            var json = JsonSerializer.Serialize(request);
            using var document = JsonDocument.Parse(json);

            Assert.Equal("terminate", document.RootElement.GetProperty("action").GetString());
        }

        [Fact]
        public void TreatUnrecognisedActionAsUnknown()
        {
            var request = JsonSerializer.Deserialize<AssessmentControlRequest>(
                "{\"action\":\"self-destruct\"}");

            Assert.Equal(AssessmentControlAction.Unknown, request.Action);
        }
    }
}
