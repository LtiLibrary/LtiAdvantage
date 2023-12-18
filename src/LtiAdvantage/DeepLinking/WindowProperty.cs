using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// How to open a resource in a new window/tab.
    /// </summary>
    public class WindowProperty
    {
        /// <summary>
        /// Height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Name of the window to open.
        /// </summary>
        [JsonPropertyName("targetName")]
        public string TargetName { get; set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Comma-separate list of features for window.open().
        /// </summary>
        [JsonPropertyName("windowFeatures")]
        public string WindowFeatures { get; set; }
    }
}
