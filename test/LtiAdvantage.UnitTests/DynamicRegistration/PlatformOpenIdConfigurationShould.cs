using System.Text.Json;
using LtiAdvantage.DynamicRegistration;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class PlatformOpenIdConfigurationShould
    {
        [Fact]
        public void ParseSpecExample()
        {
            var json = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var config = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(json);

            Assert.Equal("https://platform.example.com", config.Issuer);
            Assert.Equal("https://platform.example.com/lti/register", config.RegistrationEndpoint);
            Assert.Contains("private_key_jwt", config.TokenEndpointAuthMethodsSupported);
            Assert.NotNull(config.LtiPlatformConfiguration);
            Assert.Equal("ExamplePlatform", config.LtiPlatformConfiguration.ProductFamilyCode);
            Assert.Equal(2, config.LtiPlatformConfiguration.MessagesSupported.Count);
        }

        [Fact]
        public void RoundTrip()
        {
            var json = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var config = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(json);
            var roundTripped = JsonSerializer.Serialize(config);
            JsonAssert.Equal(json, roundTripped);
        }
    }
}
