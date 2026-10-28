using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// Represents the results returned by the Course Groups Service group sets endpoint.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-set-container-media-type.
    /// </summary>
    public class ContextGroupSetContainer
    {
        /// <summary>
        /// The ID of these results. Typically the request URL to the group sets endpoint.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The group sets in the specified context.
        /// </summary>
        [JsonPropertyName("sets")]
        public ICollection<GroupSet> Sets { get; set; }
    }
}
