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
    /// A <see cref="T:Microsoft.AspNetCore.Mvc.Controller" /> that implements a REST API for the Membership service.
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0.
    /// </summary>
    /// <remarks>
    /// Unless it is overridden, the route for this controller will be
    /// "/context/{contextid}/[controller]" named "MembershipApi".
    /// </remarks>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "membership")]
    [Route("context/{contextid}/[controller]", Name = Constants.ServiceEndpoints.MembershipService)]
    public abstract class MembershipControllerBase : ControllerBase
    {
        private readonly ILogger<MembershipControllerBase> _logger;

        protected MembershipControllerBase(ILogger<MembershipControllerBase> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get the membership.
        /// </summary>
        protected abstract Task<MembershipContainerResult> OnGetMembershipAsync(GetMembershipRequest request);

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
            _logger.LogInformation("Processing GET request.");

            try
            {
                var request = new GetMembershipRequest(contextId, limit, rlid, role);
                return await OnGetMembershipAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get membership.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
