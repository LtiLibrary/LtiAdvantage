using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class LineItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("ltiLinkId")]
        public string LtiLinkId { get; set; }

        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        [JsonProperty("scoreMaximum")]
        public float? ScoreMaximum { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
