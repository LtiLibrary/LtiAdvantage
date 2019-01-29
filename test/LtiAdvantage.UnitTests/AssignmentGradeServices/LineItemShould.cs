using System;
using System.Globalization;
using LtiAdvantage.AssignmentGradeServices;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class LineItemShould
    {
        /// <summary>
        /// Deserialize the example lineitem from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-application-vnd-ims-lis-v2-lineitem-json-representation
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItem");
            var lineitem = JsonConvert.DeserializeObject<LineItem>(referenceJson);

            Assert.Equal("https://lms.example.com/context/2923/lineitems/1", lineitem.Id);
            Assert.Equal(60.0, lineitem.ScoreMaximum);
            Assert.Equal("Chapter 5 Test", lineitem.Label);
            Assert.Equal("a-9334df-33", lineitem.ResourceId);
            Assert.Equal("grade", lineitem.Tag);
            Assert.Equal("1g3k4dlk49fk", lineitem.ResourceLinkId);
            Assert.Equal(DateTime.Parse("2018-03-06T20:05:02Z", null, DateTimeStyles.AdjustToUniversal), lineitem.StartDateTime);
            Assert.Equal(DateTime.Parse("2018-04-06T22:05:03Z", null, DateTimeStyles.AdjustToUniversal), lineitem.EndDateTime);
        }

        /// <summary>
        /// Deserialize the example lineitem from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItem");
            var lineItem = JsonConvert.DeserializeObject<LineItem>(referenceJson);
            var lineItemJson = JsonConvert.SerializeObject(lineItem);

            JsonAssertions.Equal(referenceJson, lineItemJson);
        }
    }
}
