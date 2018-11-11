using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LtiAdvantageLibrary.Membership
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MemberStatus
    {
        Active,
        Deleted,
        Inactive
    }
}
