
using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Image.
    /// </summary>
    public class ImageProperty
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Url to the image.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }
    }
}
