using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    /// <summary>Roles arrays are arrays of URI strings: an unknown role is left out, never written as null.</summary>
    public class RoleArraysShould
    {
        [Fact]
        public void LeaveUnknownRolesOut_OfTheRolesClaim()
        {
            var request = new LtiResourceLinkRequest {Roles = new[] {Role.ContextInstructor, Role.Unknown}};

            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(), request));
            var payload = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Payload;

            using var json = JsonDocument.Parse(payload.SerializeToJson());
            var roles = json.RootElement.GetProperty(Constants.LtiClaims.Roles);
            Assert.Equal(JsonValueKind.Array, roles.ValueKind);
            Assert.Equal(1, roles.GetArrayLength());
            Assert.Equal("http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor", roles[0].GetString());
            Assert.Equal(new[] {Role.ContextInstructor}, new LtiResourceLinkRequest(payload).Roles);
        }

        [Fact]
        public void LeaveUnknownRolesOut_OfAnNrpsMember()
        {
            var json = JsonSerializer.Serialize(new Member {UserId = "u", Roles = new[] {Role.Unknown, Role.ContextLearner}});

            Assert.Contains("\"roles\":[\"http://purl.imsglobal.org/vocab/lis/v2/membership#Learner\"]", json);
        }

        [Fact]
        public void ReadAnNrpsMembersRoles_IncludingOnesTheLibraryDoesNotDefine()
        {
            var member = JsonSerializer.Deserialize<Member>(
                "{\"user_id\":\"u\",\"roles\":[\"Learner\",\"https://example.edu/roles#Grader\"]}");

            Assert.Equal(new[] {Role.ContextLearner, Role.Unknown}, member.Roles);
        }
    }
}
