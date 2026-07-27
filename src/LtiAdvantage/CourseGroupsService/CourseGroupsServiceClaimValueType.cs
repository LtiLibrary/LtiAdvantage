using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// LTI claim to include in a launch message if the platform supports the
    /// Course Groups Service.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#groupsservice-claim.
    /// </summary>
    public class CourseGroupsServiceClaimValueType
    {
        /// <summary>
        /// </summary>
        public CourseGroupsServiceClaimValueType()
        {
            ServiceVersions = new[] {Version};
        }

        /// <summary>
        /// The version of this implementation.
        /// </summary>
        public const string Version = "1.0";

        /// <summary>
        /// Fully resolved URL to the context groups endpoint. Required.
        /// </summary>
        [JsonPropertyName("context_groups_url")]
        public string ContextGroupsUrl { get; set; }

        /// <summary>
        /// Fully resolved URL to the context group sets endpoint.
        /// Only present when the platform supports group sets.
        /// </summary>
        [JsonPropertyName("context_group_sets_url")]
        public string ContextGroupSetsUrl { get; set; }

        /// <summary>
        /// The scope to request to access this service. Contains
        /// <see cref="Constants.LtiScopes.Gs.ContextGroupReadonly"/>.
        /// </summary>
        [JsonPropertyName("scope")]
        public string[] Scope { get; set; }

        /// <summary>
        /// Service version. Default is <see cref="Version"/>.
        /// </summary>
        [JsonPropertyName("service_versions")]
        public string[] ServiceVersions { get; set; }
    }
}
