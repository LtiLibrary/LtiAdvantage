using System.Linq;
using System.Text.Json;
using LtiAdvantage.CourseGroupsService;
using Xunit;

namespace LtiAdvantage.UnitTests.CourseGroupsService
{
    public class ContextGroupContainerShould
    {
        /// <summary>
        /// Deserialize the example context group container from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-container-media-type.
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ContextGroupContainer");
            var container = JsonSerializer.Deserialize<ContextGroupContainer>(referenceJson);

            Assert.Equal("https://lms.example.com/class/2923/groups/", container.Id);
            Assert.Equal(3, container.Groups.Count);

            var tuesday = container.Groups.First();
            Assert.Equal("44711f9f-64af-443a-bc3d-0abbfff73790", tuesday.Id);
            Assert.Equal("Bob's Tuesday Group", tuesday.Name);
            Assert.Equal(new[] {"912cc53f-b431-447e-aff6-9d49aa9b72f2"}, tuesday.SetIds);
            Assert.Equal("marking", tuesday.Tag);
        }

        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ContextGroupContainer");
            var container = JsonSerializer.Deserialize<ContextGroupContainer>(referenceJson);
            var containerJson = JsonSerializer.Serialize(container);

            JsonAssert.Equal(referenceJson, containerJson);
        }

        /// <summary>
        /// "hidden" defaults to false but is optional, so an absent value has to stay absent
        /// rather than being reported as an explicit false.
        /// </summary>
        [Fact]
        public void LeaveHiddenUnsetWhenAbsent()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ContextGroupContainer");
            var container = JsonSerializer.Deserialize<ContextGroupContainer>(referenceJson);

            var hidden = container.Groups.Single(g => g.Id == "bf2042b4-fdad-486a-86f3-bdb84e28a099");
            var unspecified = container.Groups.Single(g => g.Id == "89230b3-a341-447e-aff6-9d354aa9b72a6");

            Assert.True(hidden.Hidden);
            Assert.Null(unspecified.Hidden);
            Assert.Null(unspecified.SetIds);
        }
    }
}
