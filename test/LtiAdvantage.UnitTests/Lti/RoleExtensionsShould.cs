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
        [InlineData("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Learner", Role.InstitutionLearner)]
        public void ParseKnownRoles(string value, Role expected)
        {
            Assert.True(RoleExtensions.TryParse(value, out var role));
            Assert.Equal(expected, role);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Unknown")]
        [InlineData("3")]
        [InlineData("-1")]
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
