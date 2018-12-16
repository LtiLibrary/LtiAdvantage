using System;
using System.Threading.Tasks;
using LtiAdvantage.Lti;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.NamesRoleProvisioningService
{
    /// <inheritdoc />
    /// <summary>
    /// Implements the Names and Role Provisioning Service membership endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.NrpsMembershipReadonly)]
    [Route("context/{contextId}/membership", Name = Constants.ServiceEndpoints.NrpsMembershipService)]
    [Route("context/{contextId}/membership.{format}")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class MembershipControllerBase : ControllerBase
    {
        /// <summary>
        /// </summary>
        protected readonly ILogger<MembershipControllerBase> Logger;

        /// <summary>
        /// </summary>
        protected MembershipControllerBase(ILogger<MembershipControllerBase> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Returns the membership.
        /// </summary>
        protected abstract Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request);

        /// <summary>
        /// Returns the membership of a context (e.g. course).
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="limit">Optional limit to the number of members to return.</param>
        /// <param name="rlid">Optional resource link filter for members with access to resource link.</param>
        /// <param name="role">Optional role filter for members that have the specified role.</param>
        /// <returns>The members.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.MembershipContainer)]
        [ProducesResponseType(typeof(MembershipContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<MembershipContainer>> GetAsync(string contextId, int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");

                if (string.IsNullOrWhiteSpace(contextId))
                {
                    Logger.LogError($"{nameof(contextId)} is missing.");
                    return BadRequest(new ProblemDetails { Title = $"{nameof(contextId)} is required." });
                }

                try
                {
                    var request = new GetMembershipRequest(contextId, limit, rlid, role);
                    return await OnGetMembershipAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot get membership.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = ex.Message,
                        Detail = ex.StackTrace
                    });
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(GetAsync)}.");
            }
        }
    }
}
