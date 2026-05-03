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
    }
}
