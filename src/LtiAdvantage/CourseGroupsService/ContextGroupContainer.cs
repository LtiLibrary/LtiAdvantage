using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// Represents the results returned by the Course Groups Service groups endpoint.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-container-media-type.
    /// </summary>
    public class ContextGroupContainer
    {
        /// <summary>
        /// The ID of these results. Typically the request URL to the groups endpoint.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The user the results were filtered to. Only present when the request
        /// included a user filter.
        /// </summary>
        [JsonPropertyName("user_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string UserId { get; set; }

        /// <summary>
        /// The groups in the specified context.
        /// </summary>
        [JsonPropertyName("groups")]
        public ICollection<Group> Groups { get; set; }
    }
}
