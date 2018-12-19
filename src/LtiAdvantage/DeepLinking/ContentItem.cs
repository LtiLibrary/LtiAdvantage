using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Base class for all content item types
    /// </summary>
    public class ContentItem : IFileItem, IHtmlItem, IImageItem, ILinkItem, ILtiLinkItem
    {
        /// <summary>
        /// Indicates the initial start and end time this activity should be made
        /// available to learners.
        /// </summary>
        [JsonProperty("available")]
        public StartEndProperty Available { get; set; }

        /// <summary>
        /// Key/value custom parameters.
        /// </summary>
        [JsonProperty("custom")]
        public Dictionary<string, string> Custom { get; set; }

        /// <summary>
        /// HTML embed code.
        /// </summary>
        [JsonProperty("embed")]
        public string Embed { get; set; }

        /// <summary>
        /// The URL will be available until this time.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// HTML fragment to embed.
        /// </summary>
        [JsonProperty("html")]
        public string Html { get; set; }

        /// <summary>
        /// URL of an icon image.
        /// </summary>
        [JsonProperty("icon")]
        public ImageProperty Icon { get; set; }

        /// <summary>
        /// iframe properties for embedding the item.
        /// </summary>
        [JsonProperty("iframe")]
        public IframeProperty Iframe { get; set; }
        
        /// <summary>
        /// Indicates this item is expected to receive scores.
        /// </summary>
        [JsonProperty("lineItem")]
        public LineItemProperty LineItem { get; set; }

        /// <summary>
        /// Start and end date and time for submissions.
        /// </summary>
        [JsonProperty("submission")]
        public StartEndProperty Submission { get; set; }

        /// <summary>
        /// Plain text description.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// URL of a thumbnail image.
        /// </summary>
        [JsonProperty("thumbnail")]
        public ImageProperty Thumbnail { get; set; }
        
        /// <summary>
        /// Plain text title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The item type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// URL of the resource.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Window properties for a new window/tab.
        /// </summary>
        [JsonProperty("window")]
        public WindowProperty Window { get; set; }
    }
}
