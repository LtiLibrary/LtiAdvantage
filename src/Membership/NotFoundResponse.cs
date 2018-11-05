using Microsoft.AspNetCore.Http;

namespace LtiAdvantageLibrary.NetCore.Membership
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:LtiAdvantageLibrary.NetCore.Membership.GetMembershipResponse" />
    /// that when executed will produce a Not Found (404) response.
    /// </summary>
    public class NotFoundResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="NotFoundResponse"/> instance.
        /// </summary>
        public NotFoundResponse()
        {
            StatusCode = StatusCodes.Status404NotFound;
        }

        /// <summary>
        /// Creates a new <see cref="NotFoundResponse"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public NotFoundResponse(object value) : this()
        {
            StatusValue = value;
        }
    }
}
