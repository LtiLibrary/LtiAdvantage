using System.Text.Json;
using LtiAdvantage.DynamicRegistration;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class ToolConfigurationShould
    {
        [Fact]
        public void ParseSpecExample()
        {
            var json = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            var tool = JsonSerializer.Deserialize<ToolConfiguration>(json);

            Assert.Equal("web", tool.ApplicationType);
            Assert.Equal("private_key_jwt", tool.TokenEndpointAuthMethod);
            Assert.NotNull(tool.LtiToolConfiguration);
            Assert.Equal("tool.example.com", tool.LtiToolConfiguration.Domain);
            Assert.Single(tool.LtiToolConfiguration.Messages);
            Assert.Equal("LtiDeepLinkingRequest", tool.LtiToolConfiguration.Messages[0].Type);
        }

        [Fact]
        public void RoundTrip()
        {
            var json = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            var tool = JsonSerializer.Deserialize<ToolConfiguration>(json);
            var roundTripped = JsonSerializer.Serialize(tool);
            JsonAssert.Equal(json, roundTripped);
        }
    }
}
