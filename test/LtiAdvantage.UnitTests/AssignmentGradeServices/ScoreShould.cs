using System;
using System.Text.Json;
using LtiAdvantage.AssignmentGradeServices;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class ScoreShould
    {
        /// <summary>
        /// Deserialize the example score from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-application-vnd-ims-lis-v1-score-json-representation
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("Score");
            var score = JsonSerializer.Deserialize<Score>(referenceJson);

            Assert.Equal(DateTime.Parse("2017-04-16T18:54:36.736+00:00"), score.TimeStamp);
            Assert.Equal(83, score.ScoreGiven);
            Assert.Equal(100, score.ScoreMaximum);
            Assert.Equal("This is exceptional work.", score.Comment);
            Assert.Equal(ActivityProgress.Completed, score.ActivityProgress);
            Assert.Equal(GradingProgress.FullyGraded, score.GradingProgress);
            Assert.Equal("5323497", score.UserId);
        }

        /// <summary>
        /// Deserialize the example score from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("Score");
            var score = JsonSerializer.Deserialize<Score>(referenceJson);
            var scoreJson = JsonSerializer.Serialize(score);

            JsonAssert.Equal(referenceJson, scoreJson);
        }

        /// <summary>
        /// A score that only reports progress has no scoreGiven or scoreMaximum. Writing 0 for
        /// them would record a grade of 0.
        /// </summary>
        [Fact]
        public void LeaveOutScoreValues_ForAProgressOnlyScore()
        {
            var score = new Score
            {
                ActivityProgress = ActivityProgress.Started,
                GradingProgress = GradingProgress.NotReady,
                TimeStamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UserId = "user-1"
            };

            var json = JsonSerializer.Serialize(score);

            Assert.DoesNotContain("scoreGiven", json);
            Assert.DoesNotContain("scoreMaximum", json);
            Assert.DoesNotContain("comment", json);
            Assert.DoesNotContain("null", json);
        }

        [Fact]
        public void ReadAProgressOnlyScore()
        {
            var score = JsonSerializer.Deserialize<Score>(
                "{\"timestamp\":\"2026-01-01T00:00:00Z\",\"activityProgress\":\"Started\",\"gradingProgress\":\"NotReady\",\"userId\":\"user-1\"}");

            Assert.Null(score.ScoreGiven);
            Assert.Null(score.ScoreMaximum);
            Assert.Equal(ActivityProgress.Started, score.ActivityProgress);
        }
    }
}
