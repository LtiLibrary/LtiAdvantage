using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// Represents a named collection of <see cref="Group"/>s within a context.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-set-container-media-type.
    /// </summary>
    public class GroupSet
    {
        /// <summary>
        /// A unique identifier for the group set. Stable for the lifetime of the set
        /// within its context. Required.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The displayable name of the group set. Required.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A string value identifying the purpose of this group set. May reference an
        /// LIS vocabulary term, for example
        /// "http://purl.imsglobal.org/vocab/lis/v2/course#CourseSection".
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Whether the group set is hidden from learners in the platform's UI.
        /// Defaults to false when not specified.
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool? Hidden { get; set; }
    }
}
