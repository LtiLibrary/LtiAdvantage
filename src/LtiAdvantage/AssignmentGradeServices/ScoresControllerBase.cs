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
    /// Implements the Assignment and Grade Services score publish service endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsScoreWriteonly)]
    [Route("context/{contextid}/lineitems/{id}/scores", Name = Constants.ServiceEndpoints.AgsScoreService)]
    [Route("context/{contextid}/lineitems/{id}/scores.{format}")]
    public abstract class ScoresControllerBase : Controller
    {
        protected readonly ILogger<ScoresControllerBase> Logger;

        protected ScoresControllerBase(ILogger<ScoresControllerBase> logger)
        {
            Logger = logger;
        }
                
        /// <summary>
        /// Post a score for a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns></returns>
        protected abstract Task<ScoreResult> OnPostScoreAsync(PostScoreRequest request);

        /// <summary>
        /// Post a score for a line item.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostAsync(string contextId, string id, [FromBody] Score score)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(PostAsync)}.");
            
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.LogError($"{nameof(id)} is missing.");
                    return BadRequest();
                }

                try
                {
                    var request = new PostScoreRequest(contextId, id, score);
                    return await OnPostScoreAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error posting score.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(PostAsync)}.");
            }
        }

        #region Convenience methods to return a properly formatted  IActionResult
        
        /// <summary>
        /// Creates a ScoreResult with 201 Created status code.
        /// </summary>
        /// <param name="score">The LineItemContainer.</param>
        /// <returns>The created <see cref="ScoreResult"/> for the response.</returns>
        public ScoreResult ScoreCreated(Score score)
            => new ScoreResult(score);

        /// <summary>
        /// Creates an empty ScoreResult with 404 status code.
        /// </summary>
        /// <returns>The created <see cref="ScoreResult"/> for the response.</returns>
        public ScoreResult ScoreNotFound()
            => new ScoreResult(StatusCodes.Status404NotFound);

        #endregion
    }
}

