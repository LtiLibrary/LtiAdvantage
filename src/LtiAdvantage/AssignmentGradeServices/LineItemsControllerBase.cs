using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Assignment and Grade Services line items endpoint.
    /// </summary>
    [Route("context/{contextId}/lineitems", Name = Constants.ServiceEndpoints.AgsLineItemsService)]
    [Route("context/{contextId}/lineitems.{format}")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class LineItemsControllerBase : ControllerBase, ILineItemsController
    {
        /// <summary>
        /// </summary>
        private readonly ILogger<ILineItemsController> _logger;
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// </summary>
        protected LineItemsControllerBase(IHostingEnvironment env, ILogger<ILineItemsController> logger)
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
        /// Get the line items for a context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line items.</returns>
        protected abstract Task<ActionResult<LineItemContainer>> OnGetLineItemsAsync(GetLineItemsRequest request);

        /// <summary>
        /// Returns the line items in a context (course).
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="resourceLinkId">Optional resource link id filter.</param>
        /// <param name="resourceId">Optional resource id filter.</param>
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
            Policy = Constants.LtiScopes.AgsLineItem + " " + Constants.LtiScopes.AgsLineItemReadonly)]
        public async Task<ActionResult<LineItemContainer>> GetAsync([Required] string contextId,
            [FromQuery(Name = "resourceLinkId")] string resourceLinkId = null,
            [FromQuery(Name = "resourceId")] string resourceId = null,
            [FromQuery] string tag = null,
            [FromQuery] int? limit = null)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(GetAsync)}.");

                try
                {
                    var request = new GetLineItemsRequest(contextId, resourceLinkId, resourceId, tag, limit);
                    return await OnGetLineItemsAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(OnGetLineItemsAsync)}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = _env.IsDevelopment()
                                 ? ex.Message + ex.StackTrace
                                 : ex.Message
                    });
                }
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetAsync)}.");
            }
        }

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsLineItem)]
        public async Task<ActionResult<LineItem>> PostAsync([Required] string contextId, [Required] [FromBody] LineItem lineItem)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(PostAsync)}.");

                try
                {
                    var request = new AddLineItemRequest(contextId, lineItem);
                    return await OnAddLineItemAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(OnAddLineItemAsync)}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = _env.IsDevelopment()
                            ? ex.Message + ex.StackTrace
                            : ex.Message
                    });
                }
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(PostAsync)}.");
            }
        }
    }
}