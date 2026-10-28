using System.Text.Json;
using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.Proctoring
{
    public class LtiStartAssessmentRequestShould
    {
        [Fact]
        public void HaveCorrectMessageTypeAndVersion()
        {
            var request = new LtiStartAssessmentRequest();

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiStartAssessment", messageType);

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);
        }

        [Fact]
        public void ParseValidLtiStartAssessmentRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiStartAssessmentRequest");
            var request = JsonSerializer.Deserialize<LtiStartAssessmentRequest>(referenceJson);
            var requestJson = JsonSerializer.Serialize(request);
            JsonAssert.Equal(referenceJson, requestJson);
        }

        [Fact]
        public void ReadProctoringClaimsFromReferenceJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiStartAssessmentRequest");
            var request = JsonSerializer.Deserialize<LtiStartAssessmentRequest>(referenceJson);

            Assert.Equal(1, request.AttemptNumber);
            Assert.Equal("ZOG9BSUgweWxVMlB1WXduZWdjOFk5dkpxOWcif", request.SessionData);
            Assert.True(request.EndAssessmentReturn);
            Assert.Equal("Jane", request.VerifiedUser.GivenName);
            Assert.Equal("Doe", request.VerifiedUser.FamilyName);
        }

        /// <summary>
        /// end_assessment_return is a JSON boolean, so the typed setter must not write it as
        /// the string "true".
        /// </summary>
        [Fact]
        public void WriteEndAssessmentReturnAsJsonBoolean()
        {
            var request = new LtiStartAssessmentRequest {EndAssessmentReturn = true};

            var json = JsonSerializer.Serialize(request);
            using var document = JsonDocument.Parse(json);

            var endAssessmentReturn = document.RootElement
                .GetProperty("https://purl.imsglobal.org/spec/lti-ap/claim/end_assessment_return");

            Assert.Equal(JsonValueKind.True, endAssessmentReturn.ValueKind);
        }
    }
}
