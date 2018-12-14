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
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class LineItemsControllerBase : ControllerBase
    {
        protected readonly ILogger<LineItemsControllerBase> Logger;

        protected LineItemsControllerBase(ILogger<LineItemsControllerBase> logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// Create a line item in the context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line item created.</returns>
        protected abstract Task<ActionResult<LineItem>> OnCreateLineItemAsync(CreateLineItemRequest request);

        /// <summary>
        /// Get the line items for a context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line items.</returns>
        protected abstract Task<ActionResult<LineItemContainer>> OnGetLineItemsAsync(GetLineItemsRequest request);

        /// <summary>
        /// Get the lineitems from a context.
        /// </summary>
        [HttpGet]
        [Produces(Constants.MediaTypes.LineItemContainer)]
        public async Task<ActionResult<LineItemContainer>> GetAsync(string contextId, 
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
        /// Create a new LineItem instance.
        /// </summary>
        [HttpPost]
        [Consumes(Constants.MediaTypes.LineItem)]
        [Produces(Constants.MediaTypes.LineItem)]
        public async Task<ActionResult<LineItem>> PostAsync(string contextId, [FromBody] LineItem lineItem)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(PostAsync)}.");

                if (!ModelState.IsValid)
                {
                    Logger.LogError($"{nameof(lineItem)} model binding failed.");
                    return BadRequest();
                }

                try
                {
                    var request = new CreateLineItemRequest(contextId, lineItem);
                    return await OnCreateLineItemAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
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