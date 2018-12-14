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
    [Route("context/{contextid}/membership", Name = Constants.ServiceEndpoints.NrpsMembershipService)]
    [Route("context/{contextid}/membership.{format}")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class MembershipControllerBase : ControllerBase
    {
        protected readonly ILogger<MembershipControllerBase> Logger;

        protected MembershipControllerBase(ILogger<MembershipControllerBase> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Get the membership.
        /// </summary>
        protected abstract Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request);

        /// <summary>
        /// Gets the membership of a context (e.g. course).
        /// </summary>
        /// <param name="contextId">The context ID (e.g. course). Required.</param>
        /// <param name="limit">Optional limit to the number of members to return.</param>
        /// <param name="rlid">Optional resource link filter for members with access to resource link.</param>
        /// <param name="role">Optional role filter for members that have the specified role.</param>
        /// <returns>The members.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.MembershipContainer)]
        public virtual async Task<ActionResult<MembershipContainer>> GetAsync(string contextId, int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");

                try
                {
                    var request = new GetMembershipRequest(contextId, limit, rlid, role);
                    return await OnGetMembershipAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot get membership.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(GetAsync)}.");
            }
        }
    }
}
