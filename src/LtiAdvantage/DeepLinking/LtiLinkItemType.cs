using System;
using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Link item type.
    /// </summary>
    public class LtiLinkItemType : ContentItemType
    {
        /// <summary>
        /// LTI link availability.
        /// </summary>
        [JsonProperty("available")]
        public StartEndPropertyType Available { get; set; }

        /// <summary>
        /// Key/value custom parameters.
        /// </summary>
        [JsonProperty("custom")]
        public string Custom { get; set; }

        /// <summary>
        /// URL of an icon image.
        /// </summary>
        [JsonProperty("icon")]
        public ImagePropertyType Icon { get; set; }

        /// <summary>
        /// Indicates this item is expected to receive scores.
        /// </summary>
        [JsonProperty("lineItem")]
        public LineItemPropertyType LineItem { get; set; }

        /// <summary>
        /// Start and end date and time for submissions.
        /// </summary>
        [JsonProperty("submission")]
        public StartEndPropertyType Submission { get; set; }

        /// <summary>
        /// URL of a thumbnail image.
        /// </summary>
        [JsonProperty("thumbnail")]
        public ImagePropertyType Thumbnail { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Item type.
        /// </summary>
        [JsonProperty("type")]
        public override ContentType Type => ContentType.LtiLink;

        /// <summary>
        /// Required URL of the resource.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
