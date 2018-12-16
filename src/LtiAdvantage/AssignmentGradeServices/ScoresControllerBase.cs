using System;
using System.ComponentModel.DataAnnotations;
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
    /// Implements the Assignment and Grade Services score publish service endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsScore)]
    [Route("context/{contextId}/lineitems/{lineItemId}/scores", Name = Constants.ServiceEndpoints.AgsScoresService)]
    [Route("context/{contextId}/lineitems/{lineItemId}/scores.{format}")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class ScoresControllerBase : ControllerBase
    {
        /// <summary>
        /// </summary>
        protected readonly ILogger<ScoresControllerBase> Logger;

        /// <summary>
        /// </summary>
        protected ScoresControllerBase(ILogger<ScoresControllerBase> logger)
        {
            Logger = logger;
        }
                
        /// <summary>
        /// Add a score to the line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns></returns>
        protected abstract Task<ActionResult<Score>> OnAddScoreAsync(AddScoreRequest request);

        /// <summary>
        /// Adds a score to a line item.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <param name="score">The score to add.</param>
        /// <returns>The new score.</returns>
        [HttpPost]
        [Consumes(Constants.MediaTypes.Score)]
        [Produces(Constants.MediaTypes.Score)]
        [ProducesResponseType(typeof(Score), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Score>> PostAsync(string contextId, string lineItemId, [Required] [FromBody] Score score)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(PostAsync)}.");
                
                if (!ModelState.IsValid)
                {
                    Logger.LogError($"{nameof(score)} model binding failed.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
            
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

                if (score == null)
                {
                    Logger.LogError($"{nameof(score)} is missing.");
                    return BadRequest(new ProblemDetails { Title = $"{nameof(score)} is required." });
                }

                try
                {
                    var request = new AddScoreRequest(contextId, lineItemId, score);
                    return await OnAddScoreAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot add score.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = ex.Message,
                        Detail = ex.StackTrace
                    });
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(PostAsync)}.");
            }
        }
    }
}

