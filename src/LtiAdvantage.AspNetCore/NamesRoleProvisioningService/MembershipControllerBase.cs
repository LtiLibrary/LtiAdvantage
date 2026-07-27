using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace LtiAdvantage.AspNetCore.NamesRoleProvisioningService
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Names and Role Provisioning Service membership endpoint.
    /// </summary>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class MembershipControllerBase : ControllerBase, IMembershipController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MembershipControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected MembershipControllerBase(IWebHostEnvironment env, ILogger<MembershipControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns the membership.
        /// </summary>
        protected abstract Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request);

        /// <summary>
        /// Returns the membership of a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="limit">Optional limit to the number of members to return.</param>
        /// <param name="rlid">Optional resource link filter for members with access to resource link.</param>
        /// <param name="role">Optional role filter for members that have the specified role.</param>
        /// <param name="groups">
        /// Optional Course Groups Service 1.0 request for each member's group enrollments.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#membership-service-with-groups.
        /// </param>
        /// <returns>The members.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.MembershipContainer)]
        [ProducesResponseType(typeof(MembershipContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
            Policy = Constants.LtiScopes.Nrps.MembershipReadonly)]
        [Route("context/{contextId}/membership", Name = Constants.ServiceEndpoints.Nrps.MembershipService)]
        [Route("context/{contextId}/membership.{format}")]
        public async Task<ActionResult<MembershipContainer>> GetMembershipAsync([Required] string contextId,
            int? limit = null, string rlid = null, Role? role = null, bool groups = false)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(GetMembershipAsync)}.");

                try
                {
                    var request = new GetMembershipRequest(contextId, limit, rlid, role, groups);
                    return await OnGetMembershipAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // The caller went away. Let the framework abort the response rather than
                    // reporting a server error the caller will never see.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(GetMembershipAsync)}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = _env.IsDevelopment()
                            ? ex.Message + ex.StackTrace
                            : ex.Message
                    });
                }
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetMembershipAsync)}.");
            }
        }
    }
}
