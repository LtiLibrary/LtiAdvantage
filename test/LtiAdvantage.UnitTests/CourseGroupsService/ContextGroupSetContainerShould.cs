using System.Linq;
using System.Text.Json;
using LtiAdvantage.CourseGroupsService;
using Xunit;

namespace LtiAdvantage.UnitTests.CourseGroupsService
{
    public class ContextGroupSetContainerShould
    {
        /// <summary>
        /// Deserialize the example context group set container from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-set-container-media-type.
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ContextGroupSetContainer");
            var container = JsonSerializer.Deserialize<ContextGroupSetContainer>(referenceJson);

            Assert.Equal("https://lms.example.com/class/2923/groups/sets", container.Id);
            Assert.Equal(3, container.Sets.Count);

            var courseSections = container.Sets.Single(s => s.Name == "Course Sections");
            Assert.Equal("6af307fc-cc7d-4867-9e6f-20c85601367a", courseSections.Id);
            Assert.Equal("http://purl.imsglobal.org/vocab/lis/v2/course#CourseSection", courseSections.Tag);
        }

        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("ContextGroupSetContainer");
            var container = JsonSerializer.Deserialize<ContextGroupSetContainer>(referenceJson);
            var containerJson = JsonSerializer.Serialize(container);

            JsonAssert.Equal(referenceJson, containerJson);
        }
    }
}
