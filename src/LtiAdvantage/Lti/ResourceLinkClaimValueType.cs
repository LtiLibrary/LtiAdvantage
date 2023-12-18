
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// A link to tool's resource from the tool's platform.
    /// </summary>
    public class ResourceLinkClaimValueType
    {
        /// <summary>
        /// This is an opaque unique within the tool platform identifier for that resource link.
        /// Uniqueness MUST be enforced within the platform.
        /// </summary>
        [JsonPropertyName("id")]
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// A plain text description of the link’s destination, suitable for display alongside the link.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// A plain text title for the resource. This is the clickable text that appears in the link.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
