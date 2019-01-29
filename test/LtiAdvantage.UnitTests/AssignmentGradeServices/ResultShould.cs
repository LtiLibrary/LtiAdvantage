using LtiAdvantage.AssignmentGradeServices;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class ResultShould
    {
        /// <summary>
        /// Deserialize the example result from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-of-an-initial-application-vnd-ims-lis-v2-resultcontainer-json-
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("Result");
            var result = JsonConvert.DeserializeObject<Result>(referenceJson);

            Assert.Equal("https://lms.example.com/context/2923/lineitems/1/results/5323497", result.Id);
            Assert.Equal("https://lms.example.com/context/2923/lineitems/1", result.ScoreOf);
            Assert.Equal("5323497", result.UserId);
            Assert.Equal(0.83, result.ResultScore);
            Assert.Equal(1, result.ResultMaximum);
            Assert.Equal("This is exceptional work.", result.Comment);
        }

        /// <summary>
        /// Deserialize the example result from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("Result");
            var result = JsonConvert.DeserializeObject<Result>(referenceJson);
            var resultJson = JsonConvert.SerializeObject(result);

            JsonAssertions.Equal(referenceJson, resultJson);
        }
    }
}
