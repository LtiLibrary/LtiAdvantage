using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantageLibrary.NetCore.Membership
{
    /// <summary>
    /// <see cref="JsonResult"/> wrapper for <see cref="MembershipContainer"/> assigns the correct
    /// <see cref="JsonResult.ContentType"/> for the Content-Type header.
    /// </summary>
    public class MembershipContainerResult : JsonResult
    {
        /// <summary>
        /// Initializes a new instance of the MembershipContainerResult class.
        /// </summary>
        /// <param name="value">The object to return.</param>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public MembershipContainerResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = LisConstants.LisMembershipContainerMediaType;
            StatusCode = statusCode;
        }
    }
}
