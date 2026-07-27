using System.Text.Json;
using LtiAdvantage.Lti;
using LtiAdvantage.Proctoring;
using Xunit;

namespace LtiAdvantage.UnitTests.Proctoring
{
    public class LtiStartProctoringRequestShould
    {
        [Fact]
        public void HaveCorrectMessageTypeAndVersion()
        {
            var request = new LtiStartProctoringRequest();

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiStartProctoring", messageType);

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);
        }

        [Fact]
        public void ParseValidLtiStartProctoringRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiStartProctoringRequest");
            var request = JsonSerializer.Deserialize<LtiStartProctoringRequest>(referenceJson);
            var requestJson = JsonSerializer.Serialize(request);
            JsonAssert.Equal(referenceJson, requestJson);
        }

        [Fact]
        public void ReadProctoringClaimsFromReferenceJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiStartProctoringRequest");
            var request = JsonSerializer.Deserialize<LtiStartProctoringRequest>(referenceJson);

            Assert.Equal(1, request.AttemptNumber);
            Assert.Equal("https://assessment.org/examgo", request.StartAssessmentUrl);
            Assert.Equal("ZOG9BSUgweWxVMlB1WXduZWdjOFk5dkpxOWcif", request.SessionData);
            Assert.Equal("video=on,audio=on,screencapture=off", request.ProctoringSettings.Data);
            Assert.Equal("https://assessment.org/acs", request.Acs.AssessmentControlUrl);
            Assert.Equal(
                new[]
                {
                    AssessmentControlAction.Terminate,
                    AssessmentControlAction.Flag,
                    AssessmentControlAction.Update
                },
                request.Acs.Actions);
        }

        /// <summary>
        /// The attempt_number claim is a JSON number in the specification's example, so the
        /// typed setter must not write it as a quoted string.
        /// </summary>
        [Fact]
        public void WriteAttemptNumberAsJsonNumber()
        {
            var request = new LtiStartProctoringRequest {AttemptNumber = 3};

            var json = JsonSerializer.Serialize(request);
            using var document = JsonDocument.Parse(json);

            var attemptNumber = document.RootElement
                .GetProperty("https://purl.imsglobal.org/spec/lti-ap/claim/attempt_number");

            Assert.Equal(JsonValueKind.Number, attemptNumber.ValueKind);
            Assert.Equal(3, attemptNumber.GetInt32());
        }

        [Fact]
        public void RoundTripAcsClaim()
        {
            var request = new LtiStartProctoringRequest
            {
                Acs = new AssessmentControlClaimValueType
                {
                    AssessmentControlUrl = "https://platform.example.com/acs",
                    Actions = new[] {AssessmentControlAction.Pause, AssessmentControlAction.Resume}
                }
            };

            var roundTripped = JsonSerializer.Deserialize<LtiStartProctoringRequest>(
                JsonSerializer.Serialize(request));

            Assert.Equal("https://platform.example.com/acs", roundTripped.Acs.AssessmentControlUrl);
            Assert.Equal(
                new[] {AssessmentControlAction.Pause, AssessmentControlAction.Resume},
                roundTripped.Acs.Actions);
        }
    }
}
