using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiAdvantageLibrary.NetCore.Membership
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MemberStatus
    {
        Active,
        Deleted,
        Inactive
    }
}
