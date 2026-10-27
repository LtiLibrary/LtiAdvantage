using System.Text.Json;
using LtiAdvantage.Lti;
using LtiAdvantage.SubmissionReview;
using Xunit;

namespace LtiAdvantage.UnitTests.SubmissionReview
{
    public class LtiSubmissionReviewRequestShould
    {
        [Fact]
        public void HaveCorrectMessageTypeAndVersion()
        {
            var request = new LtiSubmissionReviewRequest();

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiSubmissionReviewRequest", messageType);

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);
        }

        /// <summary>
        /// Pins the claim name to the spec text, so a wrong constant can't hide behind a round trip.
        /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#for-user-claim
        /// </summary>
        [Fact]
        public void UseSpecClaimName_ForForUser()
        {
            var request = new LtiSubmissionReviewRequest
            {
                ForUser = new ForUserClaimValueType { UserId = "abc-123" }
            };

            Assert.True(request.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/for_user"));
        }

        [Fact]
        public void RoundTripForUserClaim()
        {
            var request = new LtiSubmissionReviewRequest
            {
                ForUser = new ForUserClaimValueType
                {
                    UserId = "abc-123",
                    Name = "Jane Doe",
                    Email = "jane@example.edu"
                }
            };

            Assert.Equal("abc-123", request.ForUser.UserId);
            Assert.Equal("Jane Doe", request.ForUser.Name);
        }

        [Fact]
        public void ParseValidLtiSubmissionReviewRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiSubmissionReviewRequest");
            var request = JsonSerializer.Deserialize<LtiSubmissionReviewRequest>(referenceJson);
            var requestJson = JsonSerializer.Serialize(request);
            JsonAssert.Equal(referenceJson, requestJson);
        }
    }
}
