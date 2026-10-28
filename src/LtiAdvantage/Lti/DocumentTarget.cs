using System.Text.Json.Serialization;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Represents launch_presentation document_target values.
    /// </summary>
    [JsonConverter(typeof(DocumentTargetConverter))]
    public enum DocumentTarget
    {
        /// <summary>
        /// </summary>
        None = 0,

        /// <summary>
        /// The Tool is being launched within an iframe placed inside the same browser page/frame
        /// as the resource link.
        /// </summary>
        Iframe,

        /// <summary>
        /// The Tool is being launched within a new browser window or tab.
        /// </summary>
        Window,

        /// <summary>
        /// The content is embedded in the platform's page (for example a deep linking
        /// <c>link</c> item with <c>embed</c> HTML).
        /// See https://www.imsglobal.org/spec/lti-dl/v2p0#deep-linking-settings.
        /// </summary>
        /// <remarks>
        /// Only for deep linking's <c>accept_presentation_document_targets</c>. A launch's
        /// <c>launch_presentation.document_target</c> is <c>iframe</c> or <c>window</c>, so do not
        /// copy this value into <see cref="LaunchPresentationClaimValueType.DocumentTarget"/>.
        /// </remarks>
        Embed
    }
}

