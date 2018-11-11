using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantageLibrary.Membership
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="T:Microsoft.AspNetCore.Mvc.JsonResult" /> wrapper for <see cref="T:LtiAdvantageLibrary.Membership.MembershipContainer" /> assigns the correct
    /// <see cref="P:Microsoft.AspNetCore.Mvc.JsonResult.ContentType" /> for the Content-Type header.
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
            ContentType = Constants.MediaTypes.MembershipContainer;
            StatusCode = statusCode;
        }
    }
}
