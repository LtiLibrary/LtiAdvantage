using System.Collections.Generic;
using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    public class CustomPropertySubstitutionsShould
    {
        /// <summary>
        /// $Context.id is a prefix of $Context.id.history, so substituting the shorter name first
        /// leaves the tail of the longer one behind. This is the course-copy traceability
        /// variable, so getting it wrong silently breaks lineage tracking.
        /// </summary>
        [Fact]
        public void SubstituteContextIdHistory_NotTheContextIdPrefix()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {Id = "ctx-2", IdHistory = "ctx-1,ctx-0"}
            };

            var result = Substitute(substitutions, "$Context.id.history");

            Assert.Equal("ctx-1,ctx-0", result);
        }

        /// <summary>
        /// A substituted value is not scanned again: a course titled "$User.id" must not turn
        /// into the launching user's id.
        /// </summary>
        [Fact]
        public void NotExpandVariables_InsideSubstitutedValues()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {Id = "ctx-1", Title = "$User.id"},
                LtiUser = new LtiUser {Id = "user-42"}
            };

            Assert.Equal("$User.id", Substitute(substitutions, "$Context.title"));
            Assert.Equal("$User.id / user-42", Substitute(substitutions, "$Context.title / $User.id"));
        }

        /// <summary>
        /// Text that the case-insensitive pattern matches but the variable table does not know
        /// (here a Kelvin sign in place of 'k') must not fail the launch.
        /// </summary>
        [Fact]
        public void NotThrow_OnLookalikeCharactersInAVariableName()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiResourceLink = new LtiResourceLink {Id = "rl-1"}
            };

            var result = Record.Exception(() => Substitute(substitutions, "$ResourceLin\u212A.id"));

            Assert.Null(result);
        }

        [Fact]
        public void SubstituteResourceLinkIdHistory_NotTheResourceLinkIdPrefix()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiResourceLink = new LtiResourceLink {Id = "rl-2", IdHistory = "rl-1"}
            };

            var result = Substitute(substitutions, "$ResourceLink.id.history");

            Assert.Equal("rl-1", result);
        }

        /// <summary>
        /// $Context.sourcedId used to be unreachable: the sourced id was written over
        /// $Context.title, which meant the title was wrong and the sourced id was unavailable.
        /// </summary>
        [Fact]
        public void SubstituteContextSourcedIdAndTitleIndependently()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {SourcedId = "sis-course-9", Title = "Math Part 1"}
            };

            Assert.Equal("sis-course-9", Substitute(substitutions, "$Context.sourcedId"));
            Assert.Equal("Math Part 1", Substitute(substitutions, "$Context.title"));
        }

        /// <summary>
        /// The dots in a variable name are literal. Left unescaped they match any character, so
        /// text that merely resembles a variable would be substituted.
        /// </summary>
        [Fact]
        public void TreatDotsInVariableNamesAsLiteral()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiUser = new LtiUser {Id = "user-1"}
            };

            Assert.Equal("user-1", Substitute(substitutions, "$User.id"));
            Assert.Equal("$UserXid", Substitute(substitutions, "$UserXid"));
        }

        /// <summary>
        /// A substituted value is inserted literally. Values come from the platform's own data,
        /// where a "$1" or "$&amp;" is perfectly ordinary text and must not be read as a regex
        /// backreference.
        /// </summary>
        [Fact]
        public void InsertValuesContainingDollarSignsLiterally()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {Title = "Cost $1 & up"}
            };

            Assert.Equal("Cost $1 & up", Substitute(substitutions, "$Context.title"));
        }

        /// <summary>
        /// Course Groups Service 1.0 adds two variables.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#custom-parameter-substitutions.
        /// </summary>
        [Fact]
        public void SubstituteTheCourseGroupsVariables()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                CourseGroup = new CourseGroup {Id = "group-1"},
                LisMembership = new LisMembership {CourseGroupIds = "group-1,group-2"}
            };

            Assert.Equal("group-1", Substitute(substitutions, "$CourseGroup.id"));
            Assert.Equal("group-1,group-2", Substitute(substitutions, "$Membership.course.groupIds"));
        }

        /// <summary>
        /// $Membership.role is a prefix of $Membership.role.scope.mentor.
        /// </summary>
        [Fact]
        public void SubstituteTheLongerMembershipRoleVariableFirst()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LisMembership = new LisMembership {Role = "Learner", RoleScopeMentor = "mentee-1"}
            };

            Assert.Equal("mentee-1", Substitute(substitutions, "$Membership.role.scope.mentor"));
            Assert.Equal("Learner", Substitute(substitutions, "$Membership.role"));
        }

        /// <summary>
        /// A variable the platform has no value for is left alone, so the tool can tell it was
        /// not supported rather than receiving an empty string.
        /// </summary>
        [Fact]
        public void LeaveUnsupportedVariablesAlone()
        {
            var substitutions = new CustomPropertySubstitutions();

            Assert.Equal("$Context.id", Substitute(substitutions, "$Context.id"));
            Assert.Equal("$Nonsense.variable", Substitute(substitutions, "$Nonsense.variable"));
        }

        /// <summary>
        /// Variable names are matched without regard to case.
        /// </summary>
        [Fact]
        public void MatchVariableNamesCaseInsensitively()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {Id = "ctx-1"}
            };

            Assert.Equal("ctx-1", Substitute(substitutions, "$context.ID"));
        }

        [Fact]
        public void ReturnAnEmptyResultForNullInput()
        {
            Assert.Empty(new CustomPropertySubstitutions().ReplaceCustomPropertyValues(null));
        }

        [Fact]
        public void SubstituteEveryValueAndKeepTheKeys()
        {
            var substitutions = new CustomPropertySubstitutions
            {
                LtiContext = new LtiContext {Id = "ctx-1"},
                LtiUser = new LtiUser {Id = "user-1"}
            };

            var result = substitutions.ReplaceCustomPropertyValues(new Dictionary<string, string>
            {
                ["course"] = "$Context.id",
                ["who"] = "$User.id",
                ["both"] = "$Context.id/$User.id",
                ["plain"] = "no variables here"
            });

            Assert.Equal("ctx-1", result["course"]);
            Assert.Equal("user-1", result["who"]);
            Assert.Equal("ctx-1/user-1", result["both"]);
            Assert.Equal("no variables here", result["plain"]);
        }

        private static string Substitute(CustomPropertySubstitutions substitutions, string value)
        {
            return substitutions.ReplaceCustomPropertyValues(
                new Dictionary<string, string> {["key"] = value})["key"];
        }
    }
}
