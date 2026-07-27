using System.Text.Json;
using LtiAdvantage.Proctoring;
using Xunit;

namespace LtiAdvantage.UnitTests.Proctoring
{
    public class AssessmentControlResponseShould
    {
        [Theory]
        [InlineData("none", AssessmentControlStatus.None)]
        [InlineData("running", AssessmentControlStatus.Running)]
        [InlineData("paused", AssessmentControlStatus.Paused)]
        [InlineData("terminated", AssessmentControlStatus.Terminated)]
        [InlineData("complete", AssessmentControlStatus.Complete)]
        public void ParseEverySpecifiedStatus(string value, AssessmentControlStatus expected)
        {
            var response = JsonSerializer.Deserialize<AssessmentControlResponse>(
                $"{{\"status\":\"{value}\"}}");

            Assert.Equal(expected, response.Status);
        }

        /// <summary>
        /// "none" is a status the platform sends deliberately, so it must not be conflated with
        /// an absent or unrecognised status.
        /// </summary>
        [Fact]
        public void DistinguishNoneFromUnknown()
        {
            var none = JsonSerializer.Deserialize<AssessmentControlResponse>("{\"status\":\"none\"}");
            var absent = JsonSerializer.Deserialize<AssessmentControlResponse>("{}");
            var unrecognised = JsonSerializer.Deserialize<AssessmentControlResponse>("{\"status\":\"wat\"}");

            Assert.Equal(AssessmentControlStatus.None, none.Status);
            Assert.Equal(AssessmentControlStatus.Unknown, absent.Status);
            Assert.Equal(AssessmentControlStatus.Unknown, unrecognised.Status);

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(none));
            Assert.Equal("none", document.RootElement.GetProperty("status").GetString());
        }

        [Fact]
        public void ReadExtraTime()
        {
            var response = JsonSerializer.Deserialize<AssessmentControlResponse>(
                "{\"status\":\"running\",\"extra_time\":15}");

            Assert.Equal(15, response.ExtraTime);
        }
    }
}
