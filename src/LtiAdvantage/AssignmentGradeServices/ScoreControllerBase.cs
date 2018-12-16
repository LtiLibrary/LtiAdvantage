using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// A custom Assignment and Grade Services endpoint with readonly access to individual scores.
    /// </summary>
    /// <remarks>
    /// This is not part of Assignment and Grade Services spec.
    /// </remarks>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
        Policy = Constants.LtiScopes.AgsScoreReadonly + " " + Constants.LtiScopes.AgsScore)]
    [Route("context/{contextId}/lineitems/{lineItemId}/scores/{scoreId}", Name = Constants.ServiceEndpoints.AgsScoreService)]
    [Route("context/{contextId}/lineitems/{lineItemId}/scores/{scoreId}.{format}")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class ScoreControllerBase : ControllerBase
    {
        /// <summary>
        /// </summary>
        protected readonly ILogger<ScoreControllerBase> Logger;

        /// <summary>
        /// </summary>
        protected ScoreControllerBase(ILogger<ScoreControllerBase> logger)
        {
            Logger = logger;
        }
                
        /// <summary>
        /// Returns a score.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns></returns>
        protected abstract Task<ActionResult<Score>> OnGetScoreAsync(GetScoreRequest request);

        /// <summary>
        /// Returns a score.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <param name="scoreId">The score id.</param>
        /// <returns>The score.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.Score)]
        [ProducesResponseType(typeof(Score), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Score>> GetAsync(string contextId, string lineItemId, string scoreId)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");
            
                if (string.IsNullOrWhiteSpace(contextId))
                {
                    Logger.LogError($"{nameof(contextId)} is missing.");
                    return BadRequest(new ProblemDetails { Title = $"{nameof(contextId)} is required." });
                }
            
                if (string.IsNullOrWhiteSpace(lineItemId))
                {
                    Logger.LogError($"{nameof(lineItemId)} is missing.");
                    return BadRequest(new ProblemDetails { Title = $"{nameof(lineItemId)} is required." });
                }

                if (string.IsNullOrWhiteSpace(scoreId))
                {
                    Logger.LogError($"{nameof(scoreId)} is missing.");
                    return BadRequest(new ProblemDetails { Title = $"{nameof(scoreId)} is required." });
                }

                try
                {
                    var request = new GetScoreRequest(contextId, lineItemId, scoreId);
                    return await OnGetScoreAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot get score.");
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

