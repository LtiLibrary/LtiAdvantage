using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiAdvantageLibrary.NamesRoleService
{
    /// <summary>
    /// Names and Role Service member status values.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MemberStatus
    {
        /// <summary>
        /// Member is active
        /// </summary>
        Active,

        /// <summary>
        /// Member was deleted. Only used when the member is included in
        /// a difference response.
        /// </summary>
        Deleted,

        /// <summary>
        /// Member is inactive
        /// </summary>
        Inactive
    }
}
