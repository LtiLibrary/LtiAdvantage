using LtiAdvantage.AssignmentGradeServices;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class ResultContainerShould
    {
        /// <summary>
        /// Deserialize the example result container from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-of-an-initial-application-vnd-ims-lis-v2-resultcontainer-json-
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ResultContainer");
            var container = JsonConvert.DeserializeObject<ResultContainer>(referenceJson);

            Assert.Single(container);
        }
        
        /// <summary>
        /// Deserialize the example result container from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ResultContainer");
            var container = JsonConvert.DeserializeObject<ResultContainer>(referenceJson);
            var containerJson = JsonConvert.SerializeObject(container);

            JsonAssertions.Equal(referenceJson, containerJson);
        }
    }
}
