using Microsoft.AspNetCore.Http;

namespace LtiAdvantage.NamesRoleService
{
    /// <summary>
    /// Represents a GetMembership response.
    /// </summary>
    public class GetNamesRolesResponse
    {
        /// <summary>
        /// Create an empty response
        /// </summary>
        public GetNamesRolesResponse()
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
