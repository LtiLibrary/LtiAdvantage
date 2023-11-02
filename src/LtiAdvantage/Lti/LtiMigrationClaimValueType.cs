using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// A mapping of ids that have shifted with the transition to LTI 1.3, 
    /// allowing the tool to associate existing records to new LTI 1.3 identifiers.
    /// </summary>
    public class LtiMigrationClaimValueType
    {

        /// <summary>
        /// LTI 1.1 OAuth consumer key.
        /// </summary>
        [JsonPropertyName("oauth_consumer_key")]
        [Required]
        public string OAuthConsumerKey { get; set; }

        /// <summary>
        /// A signature for validating the <see cref="OAuthConsumerKey"/> to allow
        /// the tool to automate a migration.
        /// </summary>
        [JsonPropertyName("oauth_consumer_key_sign")]
        public string OAuthConsumerKeySignature { get; set; }

        /// <summary>
        /// LTI 1.1 user ID value associated with the end-user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// LTI 1.1 context ID.
        /// </summary>
        [JsonPropertyName("context_id")]
        public string ContextId { get; set; }

        /// <summary>
        /// LTI 1.1 tool consumer instance GUID.
        /// </summary>
        [JsonPropertyName("tool_consumer_instance_guid")]
        public string ToolConsumerInstanceGuid { get; set; }

        /// <summary>
        /// LTI 1.1 resource link ID.
        /// </summary>
        [JsonPropertyName("resource_link_id")]
        public string ResourceLinkId { get; set; }
    }
}
