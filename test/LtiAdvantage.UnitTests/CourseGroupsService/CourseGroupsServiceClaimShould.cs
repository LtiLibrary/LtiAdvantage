using System.Text.Json;
using LtiAdvantage.CourseGroupsService;
using LtiAdvantage.DeepLinking;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Xunit;

namespace LtiAdvantage.UnitTests.CourseGroupsService
{
    public class CourseGroupsServiceClaimShould
    {
        [Fact]
        public void DefaultToServiceVersion10()
        {
            var claim = new CourseGroupsServiceClaimValueType();

            Assert.Equal(new[] {"1.0"}, claim.ServiceVersions);
        }

        [Fact]
        public void RoundTripOnAResourceLinkRequest()
        {
            var request = new LtiResourceLinkRequest
            {
                CourseGroupsService = new CourseGroupsServiceClaimValueType
                {
                    ContextGroupsUrl = "https://www.myuniv.example.com/2344/groups",
                    ContextGroupSetsUrl = "https://www.myuniv.example.com/2344/groups/sets",
                    Scope = new[] {Constants.LtiScopes.Gs.ContextGroupReadonly}
                }
            };

            var roundTripped = JsonSerializer.Deserialize<LtiResourceLinkRequest>(
                JsonSerializer.Serialize(request));

            Assert.Equal("https://www.myuniv.example.com/2344/groups",
                roundTripped.CourseGroupsService.ContextGroupsUrl);
            Assert.Equal("https://www.myuniv.example.com/2344/groups/sets",
                roundTripped.CourseGroupsService.ContextGroupSetsUrl);
            Assert.Equal(new[] {Constants.LtiScopes.Gs.ContextGroupReadonly},
                roundTripped.CourseGroupsService.Scope);
            Assert.Equal(new[] {"1.0"}, roundTripped.CourseGroupsService.ServiceVersions);
        }

        [Fact]
        public void RoundTripOnASubmissionReviewRequest()
        {
            var request = new LtiSubmissionReviewRequest
            {
                CourseGroupsService = new CourseGroupsServiceClaimValueType
                {
                    ContextGroupsUrl = "https://www.myuniv.example.com/2344/groups"
                }
            };

            var roundTripped = JsonSerializer.Deserialize<LtiSubmissionReviewRequest>(
                JsonSerializer.Serialize(request));

            Assert.Equal("https://www.myuniv.example.com/2344/groups",
                roundTripped.CourseGroupsService.ContextGroupsUrl);
        }

        /// <summary>
        /// The three service claims are all platform-to-tool advertisements of what the tool may
        /// call, so they belong on the same message types. Declaring one of them on the shared
        /// base class instead would put it on tool-authored messages such as
        /// <see cref="LtiStartAssessmentRequest"/>, where advertising a service makes no sense.
        /// </summary>
        [Fact]
        public void BeDeclaredOnTheSameMessageTypesAsTheOtherServiceClaims()
        {
            const string groupsService = nameof(LtiResourceLinkRequest.CourseGroupsService);
            const string namesRoleService = nameof(LtiResourceLinkRequest.NamesRoleService);
            const string assignmentGradeServices = nameof(LtiResourceLinkRequest.AssignmentGradeServices);

            var platformLaunches = new[]
            {
                typeof(LtiResourceLinkRequest),
                typeof(LtiSubmissionReviewRequest),
                typeof(LtiDeepLinkingRequest)
            };
            foreach (var messageType in platformLaunches)
            {
                Assert.NotNull(messageType.GetProperty(groupsService));
                Assert.NotNull(messageType.GetProperty(namesRoleService));
                Assert.NotNull(messageType.GetProperty(assignmentGradeServices));
            }

            // Not on the shared base, and so not on the proctoring messages: the Proctoring
            // specification lists no service claim, and Start Assessment travels tool to platform.
            Assert.Null(typeof(LtiRequest).GetProperty(groupsService));
            var proctoringMessages = new[]
            {
                typeof(LtiStartProctoringRequest),
                typeof(LtiStartAssessmentRequest),
                typeof(LtiEndAssessmentRequest)
            };
            foreach (var messageType in proctoringMessages)
            {
                Assert.Null(messageType.GetProperty(groupsService));
                Assert.Null(messageType.GetProperty(namesRoleService));
                Assert.Null(messageType.GetProperty(assignmentGradeServices));
            }
        }

        [Fact]
        public void UseTheClaimUriFromTheSpecification()
        {
            var request = new LtiResourceLinkRequest
            {
                CourseGroupsService = new CourseGroupsServiceClaimValueType
                {
                    ContextGroupsUrl = "https://platform.example.com/groups"
                }
            };

            var json = JsonSerializer.Serialize(request);
            using var document = JsonDocument.Parse(json);

            Assert.True(document.RootElement.TryGetProperty(
                "https://purl.imsglobal.org/spec/lti-gs/claim/groupsservice", out var claim));
            Assert.Equal("https://platform.example.com/groups",
                claim.GetProperty("context_groups_url").GetString());
        }

        /// <summary>
        /// The Course Groups Service extends the NRPS membership response rather than defining
        /// its own membership endpoint.
        /// </summary>
        [Fact]
        public void ExposeGroupEnrollmentsOnMember()
        {
            const string json = @"{
                ""status"": ""Active"",
                ""name"": ""Jane Q. Public"",
                ""user_id"": ""0ae836b9-7fc9-4060-006f-27b2066ac545"",
                ""group_enrollments"": [
                    {""group_id"":""44711f9f-64af-443a-bc3d-0abbfff73790""},
                    {""group_id"":""bf2042b4-fdad-486a-86f3-bdb84e28a099""}
                ]
            }";

            var member = JsonSerializer.Deserialize<Member>(json);

            Assert.Equal(2, member.GroupEnrollments.Length);
            Assert.Equal("44711f9f-64af-443a-bc3d-0abbfff73790", member.GroupEnrollments[0].GroupId);
            Assert.Equal("bf2042b4-fdad-486a-86f3-bdb84e28a099", member.GroupEnrollments[1].GroupId);
        }
    }
}
