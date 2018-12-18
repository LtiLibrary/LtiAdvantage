using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Content types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        /// <summary>
        /// File.
        /// </summary>
        [EnumMember(Value = "file")]
        File = 1,

        /// <summary>
        /// HTML fragment.
        /// </summary>
        [EnumMember(Value = "html")]
        Html = 2,

        /// <summary>
        /// Image.
        /// </summary>
        [EnumMember(Value = "image")]
        Image = 3,

        /// <summary>
        /// Link.
        /// </summary>
        [EnumMember(Value = "link")]
        Link = 4,

        /// <summary>
        /// LTI link.
        /// </summary>
        [EnumMember(Value = "ltiLink")]
        LtiLink = 5
    }
}
