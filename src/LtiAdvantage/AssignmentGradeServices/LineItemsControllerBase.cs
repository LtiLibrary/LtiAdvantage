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
    /// Implements the Assignment and Grade Services line items endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsLineItem)]
    [Route("context/{contextid}/lineitems", Name = Constants.ServiceEndpoints.AgsLineItemsService)]
    [Route("context/{contextid}/lineitems.{format}")]
    public abstract class LineItemsControllerBase : Controller
    {
        protected readonly ILogger<LineItemsControllerBase> Logger;

        protected LineItemsControllerBase(ILogger<LineItemsControllerBase> logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// Get the line items for a context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line items.</returns>
        protected abstract Task<LineItemContainerResult> OnGetLineItemsAsync(GetLineItemsRequest request);

        /// <summary>
        /// Get the lineitems from a context.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId, 
            [FromQuery(Name = "lti_link_id")] string ltiLinkId = null, 
            [FromQuery(Name = "resource_id")] string resourceId = null, 
            [FromQuery] string tag = null, 
            [FromQuery] int? limit = null)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");

                try
                {
                    var request = new GetLineItemsRequest(contextId, ltiLinkId, resourceId, tag, limit);
                    return await OnGetLineItemsAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing get lineitems request.");
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
        /// Creates a LineItemContainerResult with 200 status code.
        /// </summary>
        /// <param name="lineItemContainer">The LineItemContainer.</param>
        /// <returns>The created <see cref="LineItemContainerResult"/> for the response.</returns>
        public LineItemContainerResult LineItemsOk(LineItemContainer lineItemContainer)
            => new LineItemContainerResult(lineItemContainer);

        /// <summary>
        /// Creates an empty LineItemContainerResult with 404 status code.
        /// </summary>
        /// <returns>The created <see cref="LineItemContainerResult"/> for the response.</returns>
        public LineItemContainerResult LineItemsNotFound()
            => new LineItemContainerResult(StatusCodes.Status404NotFound);

        #endregion
    }
}