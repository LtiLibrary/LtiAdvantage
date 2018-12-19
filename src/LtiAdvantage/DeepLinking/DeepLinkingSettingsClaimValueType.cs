using LtiAdvantage.Lti;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// LtiDeepLinkingRequest settings
    /// </summary>
    public class DeepLinkingSettingsClaimValueType
    {
        /// <summary>
        /// The file media types supported.
        /// </summary>
        [JsonProperty("accept_media_types")]
        public string AcceptMediaTypes { get; set; }

        /// <summary>
        /// True if the platform accepts multiple items.
        /// </summary>
        [JsonProperty("accept_multiple")]
        public bool AcceptMultiple { get; set; }

        /// <summary>
        /// The document targets supported.
        /// </summary>
        [JsonProperty("accept_presentation_document_targets")]
        public DocumentTarget[] AcceptPresentationDocumentTargets { get; set; }

        /// <summary>
        /// The content types supported.
        /// </summary>
        [JsonProperty("accept_types")]
        public string[] AcceptTypes { get; set; }

        /// <summary>
        /// True if the items are automatically created without confirmation.
        /// </summary>
        [JsonProperty("auto_create")]
        public bool AutoCreate { get; set; }

        /// <summary>
        /// An opaque value which will be returned by the tool.
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// URL to send the deep linking response.
        /// </summary>
        [JsonProperty("deep_link_return_url")]
        public string DeepLinkReturnUrl { get; set; }

        /// <summary>
        /// Default text for items returned.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Default title or alt text for items returned.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
