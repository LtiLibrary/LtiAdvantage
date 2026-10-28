using System;
using System.Text.Json;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.DeepLinking;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Xunit;

namespace LtiAdvantage.UnitTests.Lti
{
    /// <summary>
    /// Pins which shared base each message type derives from, so service claims and the
    /// proctoring claims stay declared once (issue #264).
    /// </summary>
    public class RequestHierarchyShould
    {
        [Theory]
        [InlineData(typeof(LtiResourceLinkRequest))]
        [InlineData(typeof(LtiSubmissionReviewRequest))]
        [InlineData(typeof(LtiDeepLinkingRequest))]
        public void DeriveThePlatformLaunchesFromLtiPlatformLaunchRequest(Type messageType)
        {
            Assert.True(typeof(LtiPlatformLaunchRequest).IsAssignableFrom(messageType));
        }

        [Theory]
        [InlineData(typeof(LtiStartProctoringRequest))]
        [InlineData(typeof(LtiStartAssessmentRequest))]
        [InlineData(typeof(LtiEndAssessmentRequest))]
        public void DeriveTheProctoringMessagesFromLtiProctoringRequest(Type messageType)
        {
            Assert.True(typeof(LtiProctoringRequest).IsAssignableFrom(messageType));
            Assert.False(typeof(LtiPlatformLaunchRequest).IsAssignableFrom(messageType));
        }

        [Fact]
        public void KeepResourceLinkOffTheDeepLinkingRequest()
        {
            // The Deep Linking request has every resource link launch claim except resource_link.
            Assert.Null(typeof(LtiDeepLinkingRequest).GetProperty(nameof(LtiResourceLinkRequest.ResourceLink)));
        }

        [Theory]
        [InlineData(nameof(LtiServiceLaunchRequest.AssignmentGradeServices))]
        [InlineData(nameof(LtiServiceLaunchRequest.NamesRoleService))]
        [InlineData(nameof(LtiServiceLaunchRequest.CourseGroupsService))]
        public void DeclareEachServiceClaimOnce(string property)
        {
            foreach (var messageType in new[]
                     {
                         typeof(LtiResourceLinkRequest),
                         typeof(LtiSubmissionReviewRequest),
                         typeof(LtiDeepLinkingRequest)
                     })
            {
                Assert.Equal(typeof(LtiPlatformLaunchRequest),
                    messageType.GetProperty(property)!.DeclaringType);
            }
        }

        [Theory]
        [InlineData(nameof(LtiProctoringRequest.ResourceLink))]
        [InlineData(nameof(LtiProctoringRequest.AttemptNumber))]
        [InlineData(nameof(LtiProctoringRequest.ProctoringSettings))]
        public void DeclareTheSharedProctoringClaimsOnce(string property)
        {
            foreach (var messageType in new[]
                     {
                         typeof(LtiStartProctoringRequest),
                         typeof(LtiStartAssessmentRequest),
                         typeof(LtiEndAssessmentRequest)
                     })
            {
                Assert.Equal(typeof(LtiProctoringRequest),
                    messageType.GetProperty(property)!.DeclaringType);
            }
        }

        [Fact]
        public void RoundTripServiceClaimsOnADeepLinkingRequest()
        {
            var request = new LtiDeepLinkingRequest
            {
                AssignmentGradeServices = new AssignmentGradeServicesClaimValueType
                {
                    LineItemsUrl = "https://platform.example.com/lineitems"
                },
                NamesRoleService = new NamesRoleServiceClaimValueType
                {
                    ContextMembershipUrl = "https://platform.example.com/memberships"
                }
            };

            var json = JsonSerializer.Serialize(request);
            using (var document = JsonDocument.Parse(json))
            {
                Assert.True(document.RootElement.TryGetProperty(
                    "https://purl.imsglobal.org/spec/lti-ags/claim/endpoint", out _));
                Assert.True(document.RootElement.TryGetProperty(
                    "https://purl.imsglobal.org/spec/lti-nrps/claim/namesroleservice", out _));
            }

            var parsed = JsonSerializer.Deserialize<LtiDeepLinkingRequest>(json);
            Assert.Equal("https://platform.example.com/lineitems", parsed.AssignmentGradeServices.LineItemsUrl);
            Assert.Equal("https://platform.example.com/memberships", parsed.NamesRoleService.ContextMembershipUrl);
        }
    }
}
