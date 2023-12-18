using System;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Start and end date and time.
    /// </summary>
    public class StartEndProperty
    {
        /// <summary>
        /// Optional end date and time.
        /// </summary>
        [JsonPropertyName("endDateTime")]
        public DateTime? EndDateTime { get; set; }
        
        /// <summary>
        /// Optional start date and time.
        /// </summary>
        [JsonPropertyName("startDateTime")]
        public DateTime? StartDateTime { get; set; }
    }
}
