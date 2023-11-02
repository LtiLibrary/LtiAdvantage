using System.Text.Json.Serialization;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// The context of a membership container.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The context id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The context label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }


        /// <summary>
        /// The context title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
