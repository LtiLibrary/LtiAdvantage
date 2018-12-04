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

        /// <summary>
        /// Creates an <see cref="LineItemContainerResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <param name="value">The LineItem to format in the entity body.</param>
        /// <returns>The created <see cref="LineItemContainerResult"/> for the response.</returns>
        public LineItemContainerResult Ok(LineItemContainer value)
            => new LineItemContainerResult(value);

        public LineItemContainerResult NotFound(LineItemContainer lineItem)
            => new LineItemContainerResult(StatusCodes.Status404NotFound);
    }
}