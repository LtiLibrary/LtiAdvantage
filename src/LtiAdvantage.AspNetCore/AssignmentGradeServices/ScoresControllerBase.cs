using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using LtiAdvantage.AspNetCore.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#if NETSTANDARD2_0
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace LtiAdvantage.AspNetCore.AssignmentGradeServices
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Assignment and Grade Services score publish service endpoint.
    /// </summary>
    [ApiController]
    [ServiceExceptionFilter]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class ScoresControllerBase : ControllerBase, IScoresController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ScoresControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected ScoresControllerBase(IWebHostEnvironment env, ILogger<ScoresControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }
                
        /// <summary>
        /// Add a score to the line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns></returns>
        protected abstract Task<ActionResult<Score>> OnAddScoreAsync(AddScoreRequest request);
                        
        /// <summary>
        /// Returns a score.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns></returns>
        protected abstract Task<ActionResult<Score>> OnGetScoreAsync(GetScoreRequest request);

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.Score)]
        [Route("context/{contextId}/lineitems/{lineItemId}/scores", Name = Constants.ServiceEndpoints.Ags.ScoresService)]
        [Route("context/{contextId}/lineitems/{lineItemId}/scores.{format}")]
        public async Task<ActionResult<Score>> AddScoreAsync(string contextId, string lineItemId, [Required] [FromBody] Score score)
        {
            var request = new AddScoreRequest(contextId, lineItemId, score) { CancellationToken = HttpContext.RequestAborted };
            return await OnAddScoreAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a score.
        /// </summary>
        /// <remarks>
        /// This is not part of the Assignment and Grades Services spec.
        /// </remarks>
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
            Policy = Constants.LtiScopes.Ags.ScoreReadonly + " " + Constants.LtiScopes.Ags.Score)]
        [Route("context/{contextId}/lineitems/{lineItemId}/scores/{scoreId}", Name = Constants.ServiceEndpoints.Ags.ScoreService)]
        [Route("context/{contextId}/lineitems/{lineItemId}/scores/{scoreId}.{format}")]
        public async Task<ActionResult<Score>> GetScoreAsync([Required] string contextId, 
            [Required] string lineItemId, [Required] string scoreId)
        {
            var request = new GetScoreRequest(contextId, lineItemId, scoreId) { CancellationToken = HttpContext.RequestAborted };
            return await OnGetScoreAsync(request).ConfigureAwait(false);
        }
    }
}

