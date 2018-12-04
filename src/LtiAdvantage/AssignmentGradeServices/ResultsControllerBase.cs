using System;
using System.Threading.Tasks;
using LtiAdvantage.NamesRoleProvisioningService;
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
    [Route("context/{contextid}/lineitems/{id}/results.{format}")]
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
        public async Task<IActionResult> GetAsync(string contextId, string id, 
            [FromQuery(Name = "user_id")] string userId = null, 
            [FromQuery] int? limit = null)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");
            
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.LogError($"{nameof(id)} is missing.");
                    return BadRequest();
                }

                try
                {
                    var request = new GetResultsRequest(contextId, id, userId, limit);
                    return await OnGetResultsAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing get results request.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(GetAsync)}.");
            }
        }

        #region Convenience methods to return a properly formatted  IActionResult
        
        /// <summary>
        /// Creates a ResultContainerResult with 200 status code.
        /// </summary>
        /// <param name="resultContainer">The LineItemContainer.</param>
        /// <returns>The created <see cref="MembershipContainerResult"/> for the response.</returns>
        public ResultContainerResult ResultsOk(ResultContainer resultContainer)
            => new ResultContainerResult(resultContainer);

        /// <summary>
        /// Creates an empty ResultContainerResult with 404 status code.
        /// </summary>
        /// <returns>The created <see cref="ResultContainerResult"/> for the response.</returns>
        public ResultContainerResult ResultsNotFound()
            => new ResultContainerResult(StatusCodes.Status404NotFound);

        #endregion
    }
}

