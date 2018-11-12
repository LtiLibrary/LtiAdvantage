using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiAdvantageLibrary.NamesRoleService
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MemberStatus
    {
        Active,
        Deleted,
        Inactive
    }
}
