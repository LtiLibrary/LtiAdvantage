using System.ComponentModel;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class Result
    {
        /// <summary>
        /// The endpoint url for this item.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A comment associated with this result.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Maximum result score.
        /// </summary>
        [DefaultValue(1)]
        [JsonProperty("resultMaximum")]
        public double ResultMaximum { get; set; }

        /// <summary>
        /// The line item result.
        /// </summary>
        [JsonProperty("resultScore")]
        public double ResultScore { get; set; }

        /// <summary>
        /// The line item.
        /// </summary>
        [JsonProperty("scoreOf")]
        public string ScoreOf { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
