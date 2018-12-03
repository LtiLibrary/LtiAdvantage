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
    /// Implements the Assignment and Grade Services results endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsResultReadonly)]
    [Route("context/{contextid}/lineitems/{id}/results", Name = Constants.ServiceEndpoints.AgsResultService)]
    public abstract class ResultsControllerBase : Controller
    {
        protected readonly ILogger<ResultsControllerBase> Logger;

        protected ResultsControllerBase(ILogger<ResultsControllerBase> logger)
        {
            Logger = logger;
        }
                
        /// <summary>
        /// Get the results for a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The results.</returns>
        protected abstract Task<ResultContainerResult> OnGetResultsAsync(GetResultsRequest request);

        /// <summary>
        /// Get the results for a line item.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId, string id = null, [FromQuery(Name = "user_id")] string userId = null)
        {
            Logger.LogInformation("Processing get results request.");

            try
            {
                var request = new GetResultsRequest(contextId, id, userId);
                return await OnGetResultsAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing get results request.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}

