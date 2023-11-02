using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.Utilities;
using System.Text.Json;
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
            var container = JsonSerializer.Deserialize<LineItemContainer>(referenceJson);

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
            var container = JsonSerializer.Deserialize<LineItemContainer>(referenceJson);
            var containerJson = JsonSerializer.Serialize(container);

            JsonAssert.Equal(referenceJson, containerJson);
        }
    }
}
