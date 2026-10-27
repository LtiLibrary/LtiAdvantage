using System.Text.Json;
using System.Text.Json.Nodes;
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
        /// <summary>
        /// The spec's own examples quote attempt_number in some places ("1") and not in others (1),
        /// so the claim must read either way.
        /// </summary>
        [Fact]
        public void ReadAQuotedAttemptNumber()
        {
            var json = JsonNode.Parse(TestUtils.LoadReferenceJsonFile("LtiStartProctoringRequest"));
            json[Constants.LtiClaims.AttemptNumber] = "2";

            var request = JsonSerializer.Deserialize<LtiStartProctoringRequest>(json.ToJsonString());

            Assert.Equal(2, request.AttemptNumber);
        }

        /// <summary>
        /// An action this library does not know (a later spec revision may add some) must survive
        /// a read and re-write instead of turning into null.
        /// </summary>
        [Fact]
        public void KeepUnrecognisedActions_OnARoundTrip()
        {
            const string json = """{"assessment_control_url":"https://platform.example.com/acs","actions":["pause","extend"]}""";

            var claim = JsonSerializer.Deserialize<AssessmentControlClaimValueType>(json);
            var rewritten = JsonSerializer.Serialize(claim);

            Assert.Equal(new[] {AssessmentControlAction.Pause}, claim.Actions);
            Assert.Contains("\"extend\"", rewritten);
            Assert.DoesNotContain("null", rewritten);
        }

        [Fact]
        public void NotTurnNumbersOrListsIntoActions()
        {
            const string json = """{"assessment_control_url":"https://platform.example.com/acs","actions":["pause,resume","3","99","resume"]}""";

            var claim = JsonSerializer.Deserialize<AssessmentControlClaimValueType>(json);

            Assert.Equal(new[] {AssessmentControlAction.Resume}, claim.Actions);
        }

        /// <summary>Setting a nullable claim to null removes it, instead of writing "null".</summary>
        [Fact]
        public void RemoveTheClaim_WhenSetToNull()
        {
            var request = new LtiStartProctoringRequest {AttemptNumber = 3};

            request.AttemptNumber = null;

            Assert.False(request.ContainsKey(Constants.LtiClaims.AttemptNumber));
            Assert.Null(request.AttemptNumber);
        }

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
