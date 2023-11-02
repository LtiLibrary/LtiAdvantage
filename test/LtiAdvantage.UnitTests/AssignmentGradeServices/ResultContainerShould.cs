using LtiAdvantage.AssignmentGradeServices;
using System.Text.Json;
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
            var container = JsonSerializer.Deserialize<ResultContainer>(referenceJson);

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
            var container = JsonSerializer.Deserialize<ResultContainer>(referenceJson);
            var containerJson = JsonSerializer.Serialize(container);

            JsonAssert.Equal(referenceJson, containerJson);
        }
    }
}
