using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var response = new LtiDeepLinkingResponse { ContentItems = JsonSerializer.Deserialize<ContentItem[]>(json) };

            var payload = WriteAndReadToken(response);
            var items = new LtiDeepLinkingResponse(payload).ContentItems;

            Assert.Equal(100, items[0].Icon.Width);
            Assert.Equal(100, items[0].Icon.Height);
            Assert.Equal(90, items[0].Thumbnail.Width);
            Assert.Equal(800, items[2].Width);
            Assert.Equal(600, items[2].Height);
            Assert.Equal(new DateTime(2018, 3, 6, 20, 5, 2, DateTimeKind.Utc), items[3].ExpiresAt?.ToUniversalTime());

            // What the library wrote into the token uses the spec names.
            var written = JsonNode.Parse(payload.SerializeToJson())![Constants.LtiClaims.ContentItems]!.ToJsonString();
            JsonAssert.Equal(json, written);
            Assert.Equal(KeysOf(JsonNode.Parse(json)), KeysOf(JsonNode.Parse(written)));
        }

        /// <summary>Content items written by 4.0.0 used capitalised names; those still read.</summary>
        [Fact]
        public void ReadContentItems_WrittenWithTheOldNames()
        {
            var payload = new JwtPayload();
            payload.AddClaim(new Claim(Constants.LtiClaims.ContentItems,
                "{\"type\":\"image\",\"url\":\"https://example.com/i.png\",\"Width\":800,\"Height\":600," +
                "\"ExpiresAt\":\"2018-03-06T20:05:02Z\",\"icon\":{\"url\":\"https://example.com/icon.png\",\"Width\":16,\"Height\":16}}",
                JsonClaimValueTypes.Json));

            var item = Assert.Single(new LtiDeepLinkingResponse(payload).ContentItems);

            Assert.Equal(800, item.Width);
            Assert.Equal(600, item.Height);
            Assert.NotNull(item.ExpiresAt);
            Assert.Equal(16, item.Icon.Width);
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

            var payload = WriteAndReadToken(response);

            var item = Assert.Single(new LtiDeepLinkingResponse(payload).ContentItems);
            Assert.Null(item.Icon.Width);
            Assert.Null(item.Icon.Height);
            var icon = JsonNode.Parse(payload.SerializeToJson())![Constants.LtiClaims.ContentItems]![0]!["icon"]!.AsObject();
            Assert.Equal(new[] { "url" }, icon.Select(p => p.Key).ToArray());
        }

        private static JwtPayload WriteAndReadToken(JwtPayload payload)
        {
            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(), payload));
            return new JwtSecurityTokenHandler().ReadJwtToken(jwt).Payload;
        }

        /// <summary>Every property path in the JSON, so extra or renamed keys show up.</summary>
        private static string[] KeysOf(JsonNode node, string path = "$")
        {
            return node switch
            {
                JsonObject o => o.SelectMany(p => new[] { path + "." + p.Key }.Concat(KeysOf(p.Value, path + "." + p.Key))).OrderBy(k => k).ToArray(),
                JsonArray a => a.SelectMany((n, i) => KeysOf(n, path + "[" + i + "]")).ToArray(),
                _ => Array.Empty<string>()
            };
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
