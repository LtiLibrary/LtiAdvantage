
using System.Text.Json.Serialization;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <summary>
    /// Names and Role Service member status values.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
