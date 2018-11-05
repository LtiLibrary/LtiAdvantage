using Microsoft.AspNetCore.Http;

namespace LtiAdvantageLibrary.NetCore.Membership
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:LtiAdvantageLibrary.NetCore.Membership.GetMembershipResponse" />
    /// that when executed will produce a Bad Request (400) response.
    /// </summary>
    public class BadRequestResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="BadRequestResponse"/> instance.
        /// </summary>
        public BadRequestResponse()
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        /// <summary>
        /// Creates a new <see cref="BadRequestResponse"/> instance.
        /// </summary>
        /// <param name="error">Contains the errors to be returned to the client.</param>
        public BadRequestResponse(object error) : this()
        {
            StatusValue = error;
        }
    }
}
