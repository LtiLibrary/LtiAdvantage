using Microsoft.AspNetCore.Http;

namespace LtiAdvantageLibrary.NamesRoleService
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:LtiAdvantageLibrary.NamesRoleService.GetMembershipResponse" />
    /// that when executed will produce a Unauthorized (401) response.
    /// </summary>
    public class UnauthorizedResponse : GetNamesRolesResponse
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
