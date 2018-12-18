using System;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// Link item type.
    /// </summary>
    public class LinkItemType : ContentItemType
    {
        /// <summary>
        /// HTML embed code.
        /// </summary>
        [JsonProperty("embed")]
        public string Embed { get; set; }
        
        /// <summary>
        /// URL of an icon image.
        /// </summary>
        [JsonProperty("icon")]
        public ImagePropertyType Icon { get; set; }

        /// <summary>
        /// iframe properties for embedding the item.
        /// </summary>
        [JsonProperty("iframe")]
        public IframePropertyType Iframe { get; set; }

        /// <summary>
        /// URL of a thumbnail image.
        /// </summary>
        [JsonProperty("thumbnail")]
        public ImagePropertyType Thumbnail { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// The item type.
        /// </summary>
        [JsonProperty("type")]
        public override ContentType Type => ContentType.Link;

        /// <summary>
        /// Required URL of the resource.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Window properties for a new window/tab.
        /// </summary>
        [JsonProperty("window")]
        public WindowPropertyType Window { get; set; }
    }
}
