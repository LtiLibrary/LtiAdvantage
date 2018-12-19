using Newtonsoft.Json;

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
        public int Height { get; set; }

        /// <summary>
        /// Url to the image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get; set; }
    }
}
