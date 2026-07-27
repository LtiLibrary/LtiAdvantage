using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// Represents a group within a context.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-container-media-type.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// A unique identifier for the group. Stable for the lifetime of the group
        /// within its context. Required.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The displayable name of the group. Required.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The identifiers of the <see cref="GroupSet"/>s this group belongs to.
        /// Omitted when the group belongs to no set.
        /// </summary>
        [JsonPropertyName("set_ids")]
        public string[] SetIds { get; set; }

        /// <summary>
        /// A string value identifying the purpose of this group. For example, "marking".
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Whether the group is hidden from learners in the platform's UI.
        /// Defaults to false when not specified.
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool? Hidden { get; set; }
    }
}
