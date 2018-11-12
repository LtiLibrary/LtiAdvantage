using System;
using System.Threading.Tasks;
using LtiAdvantageLibrary.Lti;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantageLibrary.NamesRoleService
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:Microsoft.AspNetCore.Mvc.Controller" /> that implements a REST API for the Membership service.
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0.
    /// </summary>
    /// <remarks>
    /// Unless it is overridden, the route for this controller will be
    /// "/context/{contextid}/[controller]" named "MembershipApi".
    /// </remarks>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("context/{contextid}/[controller]", Name = Constants.LtiClaims.NamesRoleService)]
    public abstract class NamesRoleServiceControllerBase : ControllerBase
    {
        /// <summary>
        /// Populate the <see cref="GetNamesRolesResponse"/> with the membership and set the StatusCode
        /// to signify success or failure.
        /// </summary>
        protected abstract Task<GetNamesRolesResponse> OnGetMembershipAsync(GetNamesRolesRequest request);

        /// <summary>
        /// To get the membership for a particular context, the client submits an HTTP GET 
        /// request to the resource's REST endpoint. The contextId must be embedded in the endpoint URL
        /// (e.g. /context/{contextId}/membership).
        /// </summary>
        /// <param name="contextId">The context ID (e.g. course). Required.</param>
        /// <param name="limit">Optional limit to the number of members to return.</param>
        /// <param name="rlid">Optional resource link filter for members with access to resource link.</param>
        /// <param name="role">Optional role filter for members that have the specified role.</param>
        /// <returns>Returns <see cref="MembershipContainerResult"/> if succesfull or 
        /// an <see cref="IActionResult"/> if a failure occurs.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetAsync(string contextId, int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                if (string.IsNullOrEmpty(contextId))
                {
                    return base.BadRequest($"{nameof(contextId)} is null or empty");
                }

                // Invoke OnGetMembershipAsync in the application's controller to fill in the membership
                var request = new GetNamesRolesRequest(contextId, limit, rlid, role);
                var response = await OnGetMembershipAsync(request).ConfigureAwait(false);

                // Return the result
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return new MembershipContainerResult(response.MembershipContainer);
                }
                return StatusCode(response.StatusCode, response.StatusValue);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Creates a <see cref="BadRequestResponse" /> that produces a <see cref="StatusCodes.Status400BadRequest" /> response.
        /// </summary>
        /// <returns>The created <see cref="BadRequestResponse" /> for the response.</returns>
        public new BadRequestResponse BadRequest()
        {
            return new BadRequestResponse();
        }

        /// <summary>
        /// Creates a <see cref="BadRequestResponse" /> that produces a <see cref="StatusCodes.Status400BadRequest" /> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="BadRequestResponse" /> for the response.</returns>
        public new BadRequestResponse BadRequest(object value)
        {
            return new BadRequestResponse(value);
        }

        /// <summary>
        /// Creates a <see cref="NotFoundResponse" /> that produces a <see cref="StatusCodes.Status404NotFound" /> response.
        /// </summary>
        /// <returns>The created <see cref="NotFoundResponse" /> for the response.</returns>
        public new NotFoundResponse NotFound()
        {
            return new NotFoundResponse();
        }

        /// <summary>
        /// Creates a <see cref="NotFoundResponse" /> that produces a <see cref="StatusCodes.Status404NotFound" /> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="NotFoundResponse" /> for the response.</returns>
        public new NotFoundResponse NotFound(object value)
        {
            return new NotFoundResponse(value);
        }

        /// <summary>
        /// Creates a <see cref="UnauthorizedResponse" /> that produces a <see cref="StatusCodes.Status401Unauthorized" /> response.
        /// </summary>
        /// <returns>The created <see cref="UnauthorizedResponse" /> for the response.</returns>
        public new UnauthorizedResponse Unauthorized()
        {
            return new UnauthorizedResponse();
        }

        /// <summary>
        /// Creates a <see cref="UnauthorizedResponse" /> that produces a <see cref="StatusCodes.Status401Unauthorized" /> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="UnauthorizedResponse" /> for the response.</returns>
        public UnauthorizedResponse Unauthorized(object value)
        {
            return new UnauthorizedResponse(value);
        }
    }
}
