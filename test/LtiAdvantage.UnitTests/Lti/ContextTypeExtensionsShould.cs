using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    public class ContextTypeExtensionsShould
    {
        [Fact]
        public void ConvertAContextTypeToItsUri()
        {
            Assert.Equal("http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering", ContextType.CourseOffering.ToUri());
            Assert.Null(ContextType.Unknown.ToUri());
        }

        [Theory]
        [InlineData("http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering")]
        [InlineData("urn:lti:context-type:ims/lis/CourseOffering")]
        [InlineData("CourseOffering")]
        public void ParseAContextType(string value)
        {
            Assert.True(ContextTypeExtensions.TryParse(value, out var type));
            Assert.Equal(ContextType.CourseOffering, type);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("http://example.com/other")]
        public void RejectOtherValues(string value)
        {
            Assert.False(ContextTypeExtensions.TryParse(value, out var type));
            Assert.Equal(ContextType.Unknown, type);
        }

        /// <summary>ToUri matches what the claim serializer writes.</summary>
        [Fact]
        public void ConvertEveryContextType_AsTheSerializerDoes()
        {
            foreach (ContextType type in System.Enum.GetValues(typeof(ContextType)))
            {
                if (type == ContextType.Unknown) continue;
                Assert.Equal(System.Text.Json.JsonSerializer.Serialize(type),
                    System.Text.Json.JsonSerializer.Serialize(type.ToUri() ?? type.ToString()));
            }
        }
    }
}
