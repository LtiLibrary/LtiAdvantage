using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using LtiAdvantage.AspNetCore.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace LtiAdvantage.AspNetCore.AssignmentGradeServices
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Assignment and Grade Services line items endpoint.
    /// </summary>
    [ApiController]
    [ServiceExceptionFilter]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class LineItemsControllerBase : ControllerBase, ILineItemsController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LineItemsControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected LineItemsControllerBase(IWebHostEnvironment env, ILogger<LineItemsControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Add a line item to the context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line item.</returns>
        protected abstract Task<ActionResult<LineItem>> OnAddLineItemAsync(AddLineItemRequest request);
        
        /// <summary>
        /// Deletes a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<ActionResult> OnDeleteLineItemAsync(DeleteLineItemRequest request);

        /// <summary>
        /// Get a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line item.</returns>
        protected abstract Task<ActionResult<LineItem>> OnGetLineItemAsync(GetLineItemRequest request);

        /// <summary>
        /// Get the line items for a context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line items.</returns>
        protected abstract Task<ActionResult<LineItemContainer>> OnGetLineItemsAsync(GetLineItemsRequest request);
        
        /// <summary>
        /// Updates a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<ActionResult> OnUpdateLineItemAsync(UpdateLineItemRequest request);
        
        /// <summary>
        /// Adds a line item to a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItem">The line item to add.</param>
        /// <returns>The line item added.</returns>
        [HttpPost]
        [Consumes(Constants.MediaTypes.LineItem)]
        [Produces(Constants.MediaTypes.LineItem)]
        [ProducesResponseType(typeof(LineItem), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("context/{contextId}/lineitems", Name = Constants.ServiceEndpoints.Ags.LineItemsService)]
        [Route("context/{contextId}/lineitems.{format}")]
        public async Task<ActionResult<LineItem>> AddLineItemAsync([Required] string contextId, [Required] [FromBody] LineItem lineItem)
        {
            var request = new AddLineItemRequest(contextId, lineItem) { CancellationToken = HttpContext.RequestAborted };
            return await OnAddLineItemAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a line item.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns>The result.</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("context/{contextId}/lineitems/{lineItemId}", Name = Constants.ServiceEndpoints.Ags.LineItemService)]
        [Route("context/{contextId}/lineitems/{lineItemId}.{format}")]
        public async Task<ActionResult> DeleteLineItemAsync([Required] string contextId, [Required] string lineItemId)
        {
            var request = new DeleteLineItemRequest(contextId, lineItemId) { CancellationToken = HttpContext.RequestAborted };
            return await OnDeleteLineItemAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a line item.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns>The line item.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.LineItem)]
        [ProducesResponseType(typeof(LineItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
            Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("context/{contextId}/lineitems/{lineItemId}", Name = Constants.ServiceEndpoints.Ags.LineItemService)]
        [Route("context/{contextId}/lineitems/{lineItemId}.{format}")]
        public async Task<ActionResult<LineItem>> GetLineItemAsync([Required] string contextId, [Required] string lineItemId)
        {
            var request = new GetLineItemRequest(contextId, lineItemId) { CancellationToken = HttpContext.RequestAborted };
            return await OnGetLineItemAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the line items in a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="resourceLinkId">Optional resource link id filter (<c>resource_link_id</c>).</param>
        /// <param name="resourceId">Optional resource id filter (<c>resource_id</c>).</param>
        /// <param name="tag">Optional tag filter.</param>
        /// <param name="limit">Optional limit on number of line items to return.</param>
        /// <returns>The line items.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.LineItemContainer)]
        [ProducesResponseType(typeof(LineItemContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
            Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("context/{contextId}/lineitems", Name = Constants.ServiceEndpoints.Ags.LineItemsService)]
        [Route("context/{contextId}/lineitems.{format}")]
        public async Task<ActionResult<LineItemContainer>> GetLineItemsAsync([Required] string contextId,
            [FromQuery(Name = "resource_link_id")] string resourceLinkId = null,
            [FromQuery(Name = "resource_id")] string resourceId = null,
            [FromQuery] string tag = null,
            [FromQuery] int? limit = null)
        {
            // Before 4.0.1 these were bound as resourceLinkId and resourceId; keep accepting them.
            resourceLinkId ??= Request.LegacyQueryValue("resource_link_id", "resourceLinkId");
            resourceId ??= Request.LegacyQueryValue("resource_id", "resourceId");

            var request = new GetLineItemsRequest(contextId, resourceLinkId, resourceId, tag, limit) { CancellationToken = HttpContext.RequestAborted };
            return await OnGetLineItemsAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a line item.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <param name="lineItem">The updated line item.</param>
        /// <returns>The result.</returns>
        [HttpPut]
        [Consumes(Constants.MediaTypes.LineItem)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("context/{contextId}/lineitems/{lineItemId}", Name = Constants.ServiceEndpoints.Ags.LineItemService)]
        [Route("context/{contextId}/lineitems/{lineItemId}.{format}")]
        public async Task<ActionResult> UpdateLineItemAsync([Required] string contextId, [Required] string lineItemId, [Required] LineItem lineItem)
        {
            var request = new UpdateLineItemRequest(contextId, lineItemId, lineItem) { CancellationToken = HttpContext.RequestAborted };
            return await OnUpdateLineItemAsync(request).ConfigureAwait(false);
        }
    }
}
