
using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Properties for embedded iframe.
    /// </summary>
    public class IframeProperty
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// URL to use as src of the iframe.
        /// </summary>
        [JsonPropertyName("src")]
        public string Src { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }
    }
}
