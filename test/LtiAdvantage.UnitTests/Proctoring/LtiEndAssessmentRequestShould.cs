using System.Text.Json;
using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.Proctoring
{
    public class LtiEndAssessmentRequestShould
    {
        [Fact]
        public void HaveCorrectMessageTypeAndVersion()
        {
            var request = new LtiEndAssessmentRequest();

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiEndAssessment", messageType);

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);
        }

        [Fact]
        public void ParseValidLtiEndAssessmentRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiEndAssessmentRequest");
            var request = JsonSerializer.Deserialize<LtiEndAssessmentRequest>(referenceJson);
            var requestJson = JsonSerializer.Serialize(request);
            JsonAssert.Equal(referenceJson, requestJson);
        }

        [Fact]
        public void ReadProctoringClaimsFromReferenceJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiEndAssessmentRequest");
            var request = JsonSerializer.Deserialize<LtiEndAssessmentRequest>(referenceJson);

            Assert.Equal(1, request.AttemptNumber);
            Assert.Equal("Excessive background noise", request.ErrorMessage);
            Assert.Equal("noise level exceeded threshold at 00:12:04", request.ErrorLog);
            Assert.Equal("video=on,audio=on,screencapture=off", request.ProctoringSettings.Data);
        }

        /// <summary>
        /// The proctoring errormsg/errorlog claims live under lti-ap and must not collide with
        /// the identically named Deep Linking claims under lti-dl.
        /// </summary>
        [Fact]
        public void KeepErrorClaimsSeparateFromDeepLinkingClaims()
        {
            var request = new LtiEndAssessmentRequest
            {
                ErrorMessage = "proctoring message",
                ErrorLog = "proctoring log"
            };

            var json = JsonSerializer.Serialize(request);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            Assert.Equal("proctoring message",
                root.GetProperty("https://purl.imsglobal.org/spec/lti-ap/claim/errormsg").GetString());
            Assert.Equal("proctoring log",
                root.GetProperty("https://purl.imsglobal.org/spec/lti-ap/claim/errorlog").GetString());
            Assert.False(root.TryGetProperty("https://purl.imsglobal.org/spec/lti-dl/claim/errormsg", out _));
            Assert.False(root.TryGetProperty("https://purl.imsglobal.org/spec/lti-dl/claim/errorlog", out _));
        }
    }
}
