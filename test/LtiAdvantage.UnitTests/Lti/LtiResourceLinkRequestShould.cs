using LtiAdvantage.Lti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    public class LtiResourceLinkRequestShould
    {
        /// <summary>
        /// Create minimum <see cref="LtiResourceLinkRequest"/>LtiResourceLinkRequest using constructor
        /// and required message claim setters, and test their values directly.
        /// </summary>
        [Fact]
        public void CreateValidResourceLinkRequestFromScratch()
        {
            var request = new LtiResourceLinkRequest
            {
                DeploymentId = "12345", 
                TargetLinkUri = "https://www.example.edu",
                ResourceLink = new ResourceLinkClaimValueType
                {
                    Id = "12345"
                },
                UserId = "12345",
                Lti11LegacyUserId = "12345",
                Roles = new[]{Role.ContextInstructor, Role.InstitutionInstructor}
            };

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiResourceLinkRequest", messageType);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/deployment_id", out var deploymentId));
            Assert.Equal("12345", deploymentId);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/target_link_uri", out var targetLinkUri));
            Assert.Equal("https://www.example.edu", targetLinkUri);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/resource_link", out var resourceLinkJson));
            var resourceLink = (JObject) resourceLinkJson;
            Assert.True(resourceLink.TryGetValue("id", out var id));
            Assert.Equal("12345", id);

            Assert.True(request.TryGetValue("sub", out var sub));
            Assert.Equal("12345", sub);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/lti11_legacy_user_id", out var legacyUserId));
            Assert.Equal("12345", legacyUserId);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/roles", out var rolesJson));
            var roles = ((JArray) rolesJson).ToObject<string[]>();
            Assert.Contains("http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor", roles);
            Assert.Contains("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Instructor", roles);
        }

        /// <summary>
        /// Convert a valid LtiResourceLinkRequest from IMS documentation into an <see cref="LtiResourceLinkRequest"/>,
        /// then serialize that back to JSON text and compare the JSON.
        /// </summary>
        [Fact]
        public void ParseValidLtiResourceLinkRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiResourceLinkRequest");

            var request = JsonConvert.DeserializeObject<LtiResourceLinkRequest>(referenceJson);
            var requestJson = JsonConvert.SerializeObject(request);

            JsonAssertions.Equal(referenceJson, requestJson);
        }
    }
}
