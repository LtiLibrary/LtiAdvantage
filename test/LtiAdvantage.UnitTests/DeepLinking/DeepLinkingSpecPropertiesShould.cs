using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using LtiAdvantage.DeepLinking;
using LtiAdvantage.Lti;
using LtiAdvantage.SubmissionReview;
using Xunit;

namespace LtiAdvantage.UnitTests.DeepLinking
{
    /// <summary>Deep Linking 2.0 properties: the embed target, embed HTML and lineItem extras.</summary>
    public class DeepLinkingSpecPropertiesShould
    {
        [Fact]
        public void AdvertiseTheEmbedDocumentTarget()
        {
            var request = new LtiDeepLinkingRequest
            {
                DeepLinkingSettings = new DeepLinkingSettingsClaimValueType
                {
                    AcceptPresentationDocumentTargets = new[] { DocumentTarget.Iframe, DocumentTarget.Window, DocumentTarget.Embed },
                    DeepLinkReturnUrl = "https://platform.example.com/return"
                }
            };

            var payload = RoundTrip(request);

            var targets = JsonNode.Parse(payload.SerializeToJson())![Constants.LtiClaims.DeepLinkingSettings]!
                ["accept_presentation_document_targets"]!.AsArray().Select(n => n!.GetValue<string>());
            Assert.Equal(new[] { "iframe", "window", "embed" }, targets);
            Assert.Contains(DocumentTarget.Embed,
                new LtiDeepLinkingRequest(payload).DeepLinkingSettings.AcceptPresentationDocumentTargets);
        }

        [Fact]
        public void WriteEmbedAsAnObjectWithHtml()
        {
            var response = new LtiDeepLinkingResponse
            {
                ContentItems = new ContentItem[] { new LinkItem { Url = "https://example.com", Embed = "<iframe src=\"https://example.com\"></iframe>" } }
            };

            var payload = RoundTrip(response);

            var embed = ItemJson(payload)["embed"]!.AsObject();
            Assert.Equal("<iframe src=\"https://example.com\"></iframe>", embed["html"]!.GetValue<string>());
            Assert.Equal("<iframe src=\"https://example.com\"></iframe>", new LtiDeepLinkingResponse(payload).ContentItems[0].Embed);
        }

        [Theory]
        [InlineData("{\"type\":\"link\",\"url\":\"https://example.com\",\"embed\":{\"html\":\"<p>hi</p>\"}}")]
        [InlineData("{\"type\":\"link\",\"url\":\"https://example.com\",\"embed\":\"<p>hi</p>\"}")]
        public void ReadEmbedHtml_FromTheSpecObjectAndFromTheOldString(string item)
        {
            var payload = new JwtPayload();
            payload.AddClaim(new Claim(Constants.LtiClaims.ContentItems, item, JsonClaimValueTypes.Json));

            Assert.Equal("<p>hi</p>", Assert.Single(new LtiDeepLinkingResponse(payload).ContentItems).Embed);
        }

        [Fact]
        public void LeaveOutEmbed_WhenNotSet()
        {
            var payload = RoundTrip(new LtiDeepLinkingResponse
            {
                ContentItems = new ContentItem[] { new LinkItem { Url = "https://example.com" } }
            });

            Assert.False(ItemJson(payload).ContainsKey("embed"));
        }

        [Fact]
        public void RoundTripLineItemSubmissionReviewAndGradesReleased()
        {
            var response = new LtiDeepLinkingResponse
            {
                ContentItems = new ContentItem[]
                {
                    new LtiLinkItem
                    {
                        Url = "https://tool.example.com/launch",
                        LineItem = new LineItemProperty
                        {
                            Label = "Essay",
                            ScoreMaximum = 10,
                            GradesReleased = false,
                            SubmissionReview = new SubmissionReviewProperty
                            {
                                Label = "Open my essay",
                                Url = "https://tool.example.com/review"
                            }
                        }
                    }
                }
            };

            var payload = RoundTrip(response);

            var lineItem = ItemJson(payload)["lineItem"]!.AsObject();
            Assert.False(lineItem["gradesReleased"]!.GetValue<bool>());
            Assert.Equal("https://tool.example.com/review", lineItem["submissionReview"]!["url"]!.GetValue<string>());

            var read = new LtiDeepLinkingResponse(payload).ContentItems[0].LineItem;
            Assert.False(read.GradesReleased);
            Assert.Equal("Open my essay", read.SubmissionReview.Label);
        }

        private static JwtPayload RoundTrip(JwtPayload payload)
        {
            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(), payload));
            return new JwtSecurityTokenHandler().ReadJwtToken(jwt).Payload;
        }

        private static JsonObject ItemJson(JwtPayload payload) =>
            JsonNode.Parse(payload.SerializeToJson())![Constants.LtiClaims.ContentItems]![0]!.AsObject();
    }
}
