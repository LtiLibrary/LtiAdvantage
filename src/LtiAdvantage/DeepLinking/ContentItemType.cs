using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Base class for all content item types
    /// </summary>
    public abstract class ContentItemType
    {
        /// <summary>
        /// Plain text description.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
        
        /// <summary>
        /// Plain text title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The item type.
        /// </summary>
        [JsonProperty("type")]
        public abstract ContentType Type { get; }
    }
}
