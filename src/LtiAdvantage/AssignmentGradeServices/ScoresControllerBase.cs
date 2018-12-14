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
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class ScoresControllerBase : ControllerBase
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
        protected abstract Task<ActionResult<Score>> OnCreateScoreAsync(CreateScoreRequest request);

        /// <summary>
        /// Return a score.
        /// </summary>
        /// <param name="contextId">The context (course) id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <param name="id">The score id.</param>
        /// <returns>The score.</returns>
        [HttpGet]
        [Route("context/{contextid}/lineitems/{lineItemId}/scores/{id}", Name = Constants.ServiceEndpoints.AgsScoreService)]
        [Route("context/{contextid}/lineitems/{lineItemId}/scores/{id}.{format}")]
        [Produces(Constants.MediaTypes.Score)]
        public ActionResult<Score> Get(string contextId, string lineItemId, string id)
        {
            // Not implemented.
            return null;
        }


        /// <summary>
        /// Post a score for a line item.
        /// </summary>
        [HttpPost]
        [Route("context/{contextid}/lineitems/{id}/scores", Name = Constants.ServiceEndpoints.AgsScoresService)]
        [Route("context/{contextid}/lineitems/{id}/scores.{format}")]
        [Consumes(Constants.MediaTypes.Score)]
        [Produces(Constants.MediaTypes.Score)]
        public async Task<ActionResult<Score>> PostAsync(string contextId, string id, [FromBody] Score score)
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
                    var request = new CreateScoreRequest(contextId, id, score);
                    return await OnCreateScoreAsync(request).ConfigureAwait(false);
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
    }
}

