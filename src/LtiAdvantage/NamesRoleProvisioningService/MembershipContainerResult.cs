using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <inheritdoc />
    /// <summary>
    /// A MembershipContainer <see cref="JsonResult"/>.
    /// </summary>
    public class MembershipContainerResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

        /// <inheritdoc />
        /// <summary>
        /// An empty MembershipContainer <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public MembershipContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.MembershipContainer;
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        /// A MembershipContainer <see cref="JsonResult"/> with 200 status code.
        /// </summary>
        /// <param name="membershipContainer">The membership container to return.</param>
        public MembershipContainerResult(MembershipContainer membershipContainer) : base(membershipContainer, Settings)
        {
            ContentType = Constants.MediaTypes.MembershipContainer;
            StatusCode = StatusCodes.Status200OK;
        }
    }
}
