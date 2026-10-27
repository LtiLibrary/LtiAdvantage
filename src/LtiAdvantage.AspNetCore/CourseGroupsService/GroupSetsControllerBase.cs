using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.CourseGroupsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.CourseGroupsService
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Course Groups Service group sets endpoint.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#group-sets-endpoint.
    /// </summary>
    /// <remarks>
    /// Group sets are optional. A platform that does not group its groups into sets omits
    /// <see cref="CourseGroupsServiceClaimValueType.ContextGroupSetsUrl"/> from the launch
    /// claim and need not host this endpoint.
    /// </remarks>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class GroupSetsControllerBase : ControllerBase, IGroupSetsController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GroupSetsControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected GroupSetsControllerBase(IWebHostEnvironment env, ILogger<GroupSetsControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns the group sets.
        /// </summary>
        protected abstract Task<ActionResult<ContextGroupSetContainer>> OnGetGroupSetsAsync(GetGroupSetsRequest request);

        /// <summary>
        /// Returns the group sets of a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="limit">Optional limit to the number of group sets to return.</param>
        /// <returns>The group sets.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.ContextGroupSetContainer)]
        [ProducesResponseType(typeof(ContextGroupSetContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Gs.ContextGroupReadonly)]
        [Route("context/{contextId}/groups/sets", Name = Constants.ServiceEndpoints.Gs.GroupSetsService)]
        [Route("context/{contextId}/groups/sets.{format}")]
        public async Task<ActionResult<ContextGroupSetContainer>> GetGroupSetsAsync([Required] string contextId,
            int? limit = null)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(GetGroupSetsAsync)}.");

                try
                {
                    var request = new GetGroupSetsRequest(contextId, limit);
                    return await OnGetGroupSetsAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // The caller went away. Let the framework abort the response rather than
                    // reporting a server error the caller will never see.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(GetGroupSetsAsync)}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = _env.IsDevelopment()
                            ? ex.Message + ex.StackTrace
                            : null // Exception messages can reveal configuration; Development only.
                    });
                }
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetGroupSetsAsync)}.");
            }
        }
    }
}
