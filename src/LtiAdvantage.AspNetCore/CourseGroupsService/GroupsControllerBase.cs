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
    /// Implements the Course Groups Service groups endpoint.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#groups-endpoint.
    /// </summary>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class GroupsControllerBase : ControllerBase, IGroupsController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GroupsControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected GroupsControllerBase(IWebHostEnvironment env, ILogger<GroupsControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns the groups.
        /// </summary>
        protected abstract Task<ActionResult<ContextGroupContainer>> OnGetGroupsAsync(GetGroupsRequest request);

        /// <summary>
        /// Returns the groups of a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="userId">Optional user filter for groups the user belongs to.</param>
        /// <param name="limit">Optional limit to the number of groups to return.</param>
        /// <returns>The groups.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.ContextGroupContainer)]
        [ProducesResponseType(typeof(ContextGroupContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Gs.ContextGroupReadonly)]
        [Route("context/{contextId}/groups", Name = Constants.ServiceEndpoints.Gs.GroupsService)]
        [Route("context/{contextId}/groups.{format}")]
        public async Task<ActionResult<ContextGroupContainer>> GetGroupsAsync([Required] string contextId,
            [FromQuery(Name = "user_id")] string userId = null, int? limit = null)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(GetGroupsAsync)}.");

                try
                {
                    var request = new GetGroupsRequest(contextId, userId, limit);
                    return await OnGetGroupsAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // The caller went away. Let the framework abort the response rather than
                    // reporting a server error the caller will never see.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(GetGroupsAsync)}.");
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
                _logger.LogDebug($"Exiting {nameof(GetGroupsAsync)}.");
            }
        }
    }
}
