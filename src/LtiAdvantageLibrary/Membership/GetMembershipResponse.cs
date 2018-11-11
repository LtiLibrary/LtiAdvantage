using Microsoft.AspNetCore.Http;

namespace LtiAdvantageLibrary.Membership
{
    /// <summary>
    /// Represents a GetMembership response.
    /// </summary>
    public class GetMembershipResponse
    {
        /// <summary>
        /// Create an empty response
        /// </summary>
        public GetMembershipResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the MembershipContainer.
        /// </summary>
        public MembershipContainer MembershipContainer { get; set; }

        /// <summary>
        /// Get or set the HTTP status code representing the success or failure of the membership
        /// service request.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Get or set the HTTP status value representing the success or failure of the membership
        /// service request.
        /// </summary>
        public object StatusValue { get; set; }
    }
}
