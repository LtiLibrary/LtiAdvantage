using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using LtiAdvantage.DeepLinking;
using Xunit;

namespace LtiAdvantage.UnitTests.DeepLinking
{
    public class LtiDeepLinkingResponseShould
    {
        private static LtiDeepLinkingResponse CreateResponse() => new()
        {
            DeploymentId = "deployment",
            ContentItems = new ContentItem[]
            {
                new LtiLinkItem
                {
                    Title = "First",
                    Text = "First activity",
                    Url = "https://tool.example.com/launch",
                    Custom = new Dictionary<string, string> { ["activity_id"] = "1" }
                },
                new LtiLinkItem { Title = "Second", Url = "https://tool.example.com/launch" }
            }
        };

        [Fact]
        public void ReadContentItems_FromTheSamePayload()
        {
            var response = CreateResponse();

            AssertContentItems(response.ContentItems);
        }

        [Fact]
        public void ReadContentItems_AfterJwtRoundTrip()
        {
            // The platform receives the response as a JWT and reads it back from the decoded payload.
            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(), CreateResponse()));
            var payload = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Payload;

            AssertContentItems(new LtiDeepLinkingResponse(payload).ContentItems);
        }

        private static void AssertContentItems(ContentItem[] items)
        {
            Assert.NotNull(items);
            Assert.Equal(2, items.Length);
            Assert.Equal(Constants.ContentItemTypes.LtiLink, items[0].Type);
            Assert.Equal("First", items[0].Title);
            Assert.Equal("First activity", items[0].Text);
            Assert.Equal("1", items[0].Custom["activity_id"]);
            Assert.Equal("Second", items[1].Title);
        }
    }
}
