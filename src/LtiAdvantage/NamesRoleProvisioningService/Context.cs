using LtiAdvantage.Lti;
using Newtonsoft.Json;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// The context of a membership container.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The context id.
        /// Corresponds to <code>Id</code> in <see cref="LtiResourceLinkRequest.Context"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The context label.
        /// Corresponds to <code>Label</code> in <see cref="LtiResourceLinkRequest.Context"/>.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }


        /// <summary>
        /// The context title.
        /// Corresponds to <code>Title</code> in <see cref="LtiResourceLinkRequest.Context"/>.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
