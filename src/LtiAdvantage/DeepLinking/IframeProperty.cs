using Newtonsoft.Json;

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
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// URL to use as src of the iframe.
        /// </summary>
        [JsonProperty("src")]
        public string Src { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
