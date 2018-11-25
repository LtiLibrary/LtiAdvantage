using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="T:Microsoft.AspNetCore.Mvc.JsonResult" /> wrapper for
    /// <see cref="T:LtiAdvantage.NamesRoleProvisioningService.MembershipContainer" /> assigns the correct
    /// <see cref="P:Microsoft.AspNetCore.Mvc.JsonResult.ContentType" /> for the Content-Type header.
    /// </summary>
    public class MembershipContainerResult : JsonResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MembershipContainerResult class.
        /// </summary>
        /// <param name="value">The membership container to return.</param>
        public MembershipContainerResult(MembershipContainer value) : base(value)
        {
            ContentType = Constants.MediaTypes.MembershipContainer;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MembershipContainerResult class.
        /// </summary>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public MembershipContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.MembershipContainer;
            StatusCode = statusCode;
        }
    }
}
