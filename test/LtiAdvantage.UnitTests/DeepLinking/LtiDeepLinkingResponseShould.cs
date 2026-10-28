using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        /// <summary>
        /// Content items shaped like the Deep Linking 2.0 examples read into the model and come
        /// back out with the same property names.
        /// </summary>
        [Fact]
        public void RoundTripTheSpecContentItems()
        {
            var json = TestUtils.LoadReferenceJsonFile("DeepLinkingContentItems");
            var payload = new JwtPayload();
            foreach (var item in JsonNode.Parse(json)!.AsArray())
            {
                payload.AddClaim(new System.Security.Claims.Claim(Constants.LtiClaims.ContentItems,
                    item!.ToJsonString(), System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes.Json));
            }

            var items = new LtiDeepLinkingResponse(payload).ContentItems;

            Assert.Equal(100, items[0].Icon.Width);
            Assert.Equal(100, items[0].Icon.Height);
            Assert.Equal(90, items[0].Thumbnail.Width);
            Assert.Equal(800, items[2].Width);
            Assert.Equal(600, items[2].Height);
            Assert.Equal(new DateTime(2018, 3, 6, 20, 5, 2, DateTimeKind.Utc), items[3].ExpiresAt?.ToUniversalTime());

            var written = JsonSerializer.Serialize(new LtiDeepLinkingResponse { ContentItems = items }.ContentItems,
                new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
            JsonAssert.Equal(json, written);
            Assert.DoesNotContain("\"Width\"", written);
            Assert.DoesNotContain("\"Height\"", written);
            Assert.DoesNotContain("\"ExpiresAt\"", written);
        }

        /// <summary>An icon without dimensions is written without them, not as 0 x 0.</summary>
        [Fact]
        public void NotWriteZeroDimensions_ForAnIconWithout()
        {
            var response = new LtiDeepLinkingResponse
            {
                ContentItems = new ContentItem[]
                {
                    new LinkItem { Url = "https://tool.example.com", Icon = new ImageProperty { Url = "https://tool.example.com/icon.png" } }
                }
            };
            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(), response));
            var payload = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Payload;

            var item = Assert.Single(new LtiDeepLinkingResponse(payload).ContentItems);
            Assert.Null(item.Icon.Width);
            Assert.Null(item.Icon.Height);
            Assert.DoesNotContain("eight", payload.SerializeToJson());
            Assert.DoesNotContain("idth", payload.SerializeToJson());
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
