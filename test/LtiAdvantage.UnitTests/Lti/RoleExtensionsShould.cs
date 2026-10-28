using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    public class RoleExtensionsShould
    {
        [Theory]
        [InlineData("http://purl.imsglobal.org/vocab/lis/v2/membership#Learner", Role.ContextLearner)]
        [InlineData("Learner", Role.ContextLearner)]
        [InlineData("ContextLearner", Role.ContextLearner)]
        [InlineData(" Instructor ", Role.ContextInstructor)]
        [InlineData("contextlearner", Role.ContextLearner)]
        [InlineData("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Learner", Role.InstitutionLearner)]
        public void ParseKnownRoles(string value, Role expected)
        {
            Assert.True(RoleExtensions.TryParse(value, out var role));
            Assert.Equal(expected, role);
        }

        [Theory]
        [InlineData(Role.ContextLearner, "http://purl.imsglobal.org/vocab/lis/v2/membership#Learner")]
        [InlineData(Role.InstitutionLearner, "http://purl.imsglobal.org/vocab/lis/v2/institution/person#Learner")]
        [InlineData(Role.Unknown, null)]
        public void ConvertARoleToItsUri(Role role, string expected)
        {
            Assert.Equal(expected, role.ToUri());
        }

        /// <summary>ToUri matches what the claim serializer writes, so either can be used.</summary>
        [Fact]
        public void ConvertEveryRole_AsTheSerializerDoes()
        {
            foreach (Role role in System.Enum.GetValues(typeof(Role)))
            {
                if (role == Role.Unknown) continue;
                var json = System.Text.Json.JsonSerializer.Serialize(role);
                Assert.Equal(json, System.Text.Json.JsonSerializer.Serialize(role.ToUri() ?? role.ToString()));
                Assert.True(RoleExtensions.TryParse(role.ToUri() ?? role.ToString(), out var parsed));
                Assert.Equal(role, parsed);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Unknown")]
        [InlineData("3")]
        [InlineData("-1")]
        [InlineData("+3")]
        [InlineData("learner")]
        [InlineData("ContextLearner, ContextInstructor")]
        [InlineData("http://example.com/role")]
        public void RejectOtherValues(string value)
        {
            Assert.False(RoleExtensions.TryParse(value, out var role));
            Assert.Equal(Role.Unknown, role);
        }
    }
}
