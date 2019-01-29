using System;
using LtiAdvantage.AssignmentGradeServices;
using Newtonsoft.Json;
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
            var score = JsonConvert.DeserializeObject<Score>(referenceJson);

            Assert.Equal(DateTime.Parse("2017-04-16T18:54:36.736+00:00"), score.TimeStamp);
            Assert.Equal(83, score.ScoreGiven);
            Assert.Equal(100, score.ScoreMaximum);
            Assert.Equal("This is exceptional work.", score.Comment);
            Assert.Equal(ActivityProgress.Completed, score.ActivityProgress);
            Assert.Equal(GradingProgess.FullyGraded, score.GradingProgress);
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
            var score = JsonConvert.DeserializeObject<Score>(referenceJson);
            var scoreJson = JsonConvert.SerializeObject(score);

            JsonAssertions.Equal(referenceJson, scoreJson);
        }
    }
}
