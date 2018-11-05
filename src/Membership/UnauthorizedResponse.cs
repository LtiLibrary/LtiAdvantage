using Microsoft.AspNetCore.Http;

namespace LtiAdvantageLibrary.NetCore.Membership
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:LtiAdvantageLibrary.NetCore.Membership.GetMembershipResponse" />
    /// that when executed will produce a Unauthorized (401) response.
    /// </summary>
    public class UnauthorizedResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="UnauthorizedResponse"/> instance.
        /// </summary>
        public UnauthorizedResponse()
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }

        /// <summary>
        /// Creates a new <see cref="UnauthorizedResponse"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public UnauthorizedResponse(object value) : this()
        {
            StatusValue = value;
        }
    }
}
