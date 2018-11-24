using Microsoft.AspNetCore.Http;

namespace LtiAdvantage.NamesRoleService
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:LtiAdvantage.NamesRoleService.GetMembershipResponse" />
    /// that when executed will produce a Not Found (404) response.
    /// </summary>
    public class NotFoundResponse : GetNamesRolesResponse
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
