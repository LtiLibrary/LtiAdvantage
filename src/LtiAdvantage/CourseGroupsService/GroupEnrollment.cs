using System.Text.Json.Serialization;

namespace LtiAdvantage.CourseGroupsService
{
    /// <summary>
    /// A single group membership reported against a member of a context.
    /// Included in the Names and Role Provisioning Service membership response when
    /// the caller requests groups.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#membership-service-with-groups.
    /// </summary>
    public class GroupEnrollment
    {
        /// <summary>
        /// The <see cref="Group.Id"/> of a group the member belongs to.
        /// </summary>
        [JsonPropertyName("group_id")]
        public string GroupId { get; set; }
    }
}
