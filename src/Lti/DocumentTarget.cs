using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming
namespace LtiAdvantageLibrary.NetCore.Lti
{
    /// <summary>
    /// Represents launch_presentation document_target values.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentTarget
    {
        /// <summary>
        /// The Tool is being launched within an iframe placed inside the same browser page/frame
        /// as the resource link.
        /// </summary>
        iframe,

        /// <summary>
        /// The Tool is being launched within a new browser window or tab.
        /// </summary>
        window
    }
}
// ReSharper restore InconsistentNaming
