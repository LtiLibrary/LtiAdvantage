using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// Image item type.
    /// </summary>
    public class ImageItemType : ContentItemType
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// URL of an icon image.
        /// </summary>
        [JsonProperty("icon")]
        public ImagePropertyType Icon { get; set; }

        /// <summary>
        /// URL of a thumbnail image.
        /// </summary>
        [JsonProperty("thumbnail")]
        public ImagePropertyType Thumbnail { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Image item type.
        /// </summary>
        [JsonProperty("type")]
        public override ContentType Type => ContentType.Image;

        /// <summary>
        /// Required URL of the resource.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get; set; }
    }
}
