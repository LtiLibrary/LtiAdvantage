using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// LTI claim to include in the LtiResourceLinkRequest if the platform
    /// supports Assignment and Grade Services.
    /// </summary>
    public class AssignmentGradeServicesClaimValueType
    {
        /// <summary>
        /// The list of scopes the tool may ask for when requesting the access token.
        /// </summary>
        [JsonPropertyName("scope")]
        public IList<string> Scope { get; set; }

        /// <summary>
        /// The fully resolved URL to the line item endpoint associated with the resource link.
        /// </summary>
        [JsonPropertyName("lineitem")]
        public string LineItemUrl { get; set; }

        /// <summary>
        /// The fully resolved URL to the line items endpoint associated with the context.
        /// </summary>
        [JsonPropertyName("lineitems")]
        public string LineItemsUrl { get; set; }
    }
}
