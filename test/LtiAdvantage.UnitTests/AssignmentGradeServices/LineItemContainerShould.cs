using LtiAdvantage.AssignmentGradeServices;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class LineItemContainerShould
    {
        /// <summary>
        /// Deserialize the example lineitem container from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-application-vnd-ims-lis-v2-lineitemcontainer-json-representation
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItemContainer");
            var container = JsonConvert.DeserializeObject<LineItemContainer>(referenceJson);

            Assert.Equal(3, container.Count);
        }
        
        /// <summary>
        /// Deserialize the example lineitem container from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItemContainer");
            var container = JsonConvert.DeserializeObject<LineItemContainer>(referenceJson);
            var containerJson = JsonConvert.SerializeObject(container);

            JsonAssertions.Equal(referenceJson, containerJson);
        }
    }
}
