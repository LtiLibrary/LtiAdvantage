﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="IFileItem" />
    /// <inheritdoc cref="IHtmlItem" />
    /// <inheritdoc cref="IImageItem" />
    /// <inheritdoc cref="ILinkItem" />
    /// <inheritdoc cref="ILtiLinkItem" />
    /// <summary>
    /// Base class for all content item types
    /// </summary>
    public class ContentItem : IFileItem, IHtmlItem, IImageItem, ILinkItem, ILtiLinkItem
    {
        /// <inheritdoc />
        /// <summary>
        /// Indicates the initial start and end time this activity should be made
        /// available to learners.
        /// </summary>
        [JsonPropertyName("available")]
        public StartEndProperty Available { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Key/value custom parameters.
        /// </summary>
        [JsonPropertyName("custom")]
        public Dictionary<string, string> Custom { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// HTML embed code.
        /// </summary>
        [JsonPropertyName("embed")]
        public string Embed { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// The URL will be available until this time.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int? Height { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// HTML fragment to embed.
        /// </summary>
        [JsonPropertyName("html")]
        public string Html { get; set; }

        /// <inheritdoc cref="IFileItem" />
        /// <inheritdoc cref="IImageItem" />
        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// URL of an icon image.
        /// </summary>
        [JsonPropertyName("icon")]
        public ImageProperty Icon { get; set; }

        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// iframe properties for embedding the item.
        /// </summary>
        [JsonPropertyName("iframe")]
        public IframeProperty Iframe { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// Indicates this item is expected to receive scores.
        /// </summary>
        [JsonPropertyName("lineItem")]
        public LineItemProperty LineItem { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Start and end date and time for submissions.
        /// </summary>
        [JsonPropertyName("submission")]
        public StartEndProperty Submission { get; set; }

        /// <inheritdoc cref="IFileItem"/>
        /// <inheritdoc cref="IHtmlItem"/>
        /// <inheritdoc cref="IImageItem"/>
        /// <inheritdoc cref="ILinkItem"/>
        /// <inheritdoc cref="ILtiLinkItem"/>
        /// <summary>
        /// Plain text description.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <inheritdoc cref="IFileItem" />
        /// <inheritdoc cref="IImageItem" />
        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// URL of a thumbnail image.
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public ImageProperty Thumbnail { get; set; }
        
        /// <inheritdoc cref="IFileItem" />
        /// <inheritdoc cref="IImageItem" />
        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// Plain text title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The item type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <inheritdoc cref="IFileItem" />
        /// <inheritdoc cref="IImageItem" />
        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// URL of the resource.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int? Width { get; set; }

        /// <inheritdoc cref="ILinkItem" />
        /// <inheritdoc cref="ILtiLinkItem" />
        /// <summary>
        /// Window properties for a new window/tab.
        /// </summary>
        [JsonPropertyName("window")]
        public WindowProperty Window { get; set; }
    }
}
