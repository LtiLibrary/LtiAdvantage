using System.Linq;
using LtiAdvantage.Lti;
using LtiAdvantage.Utilities;
using System.Text.Json;
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
            var resourceLink = (JsonElement) resourceLinkJson;
            Assert.True(resourceLink.TryGetString("id", out var id));
            Assert.Equal("12345", id);

            Assert.True(request.TryGetValue("sub", out var sub));
            Assert.Equal("12345", sub);

            Assert.True(request.TryGetValue("https://purl.imsglobal.org/spec/lti/claim/roles", out var rolesJson));
            var roles = ((JsonElement)rolesJson).ToStringList();
            Assert.Contains("http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor", roles);
            Assert.Contains("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Instructor", roles);
        }
        
        [Fact]
        public void CreateValidResourceLinkRequestFromScratch_WithSingleRole()
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
                Roles = new[]{Role.InstitutionInstructor}
            };

            var rolesClaimOld = request.Claims.Single(x => x.Type == "https://purl.imsglobal.org/spec/lti/claim/roles");
            Assert.Equal("http://www.w3.org/2001/XMLSchema#string", rolesClaimOld.ValueType);
            
            var rolesClaim = request.IssuedClaims.Single(x => x.Type == "https://purl.imsglobal.org/spec/lti/claim/roles");
            Assert.Equal("JSON_ARRAY", rolesClaim.ValueType);
        }

        /// <summary>
        /// Convert a valid LtiResourceLinkRequest from IMS documentation into an <see cref="LtiResourceLinkRequest"/>,
        /// then serialize that back to JSON text and compare the JSON.
        /// </summary>
        [Fact]
        public void ParseValidLtiResourceLinkRequest()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LtiResourceLinkRequest");

            var request = JsonSerializer.Deserialize<LtiResourceLinkRequest>(referenceJson);
            var requestJson = JsonSerializer.Serialize(request);

            JsonAssert.Equal(referenceJson, requestJson);
        }
    }
}
