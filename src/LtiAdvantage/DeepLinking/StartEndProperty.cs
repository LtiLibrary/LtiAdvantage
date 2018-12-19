using System;
using Newtonsoft.Json;

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
        [JsonProperty("endDateTime")]
        public DateTime? EndDateTime { get; set; }
        
        /// <summary>
        /// Optional start date and time.
        /// </summary>
        [JsonProperty("startDateTime")]
        public DateTime? StartDateTime { get; set; }
    }
}
