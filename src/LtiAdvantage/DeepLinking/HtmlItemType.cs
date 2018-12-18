using Newtonsoft.Json;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// HTML fragment to embed.
    /// </summary>
    public class HtmlItemType : ContentItemType
    {
        /// <summary>
        /// HTML fragment to embed.
        /// </summary>
        [JsonProperty("html")]
        public string Html { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// HTML fragment item type.
        /// </summary>
        [JsonProperty("type")]
        public override ContentType Type => ContentType.Html;
    }
}
