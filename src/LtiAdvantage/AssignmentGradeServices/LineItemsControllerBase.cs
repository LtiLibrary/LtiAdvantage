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
    /// An <see cref="T:Microsoft.AspNetCore.Mvc.Controller" /> that implements 
    /// "A REST API for LineItem Resources in multiple formats, Internal Draft 2.1"
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "lineitems")]
    [Route("context/{contextid}/[controller]/{id?}", Name = Constants.ServiceEndpoints.LineItemsService)]
    public abstract class LineItemsControllerBase : Controller
    {
        protected readonly ILogger<LineItemsControllerBase> Logger;

        protected LineItemsControllerBase(ILogger<LineItemsControllerBase> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Create a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line item created.</returns>
        protected abstract Task<LineItemResult> OnCreateLineItemAsync(PostLineItemRequest request);
        
        /// <summary>
        /// Get the line items for a context.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line items.</returns>
        protected abstract Task<LineItemContainerResult> OnGetLineItemsAsync(GetLineItemsRequest request);

        /// <summary>
        /// Delete a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<IActionResult> OnDeleteLineItemAsync(DeleteLineItemRequest request);

        /// <summary>
        /// Get a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The line item.</returns>
        protected abstract Task<LineItemResult> OnGetLineItemAsync(GetLineItemRequest request);

        /// <summary>
        /// Update a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<IActionResult> OnUpdateLineItemAsync(PutLineItemRequest request);

        /// <summary>
        /// Delete a particular LineItem instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string id)
        {
            Logger.LogInformation("Processing delete lineitem request.");

            if (string.IsNullOrWhiteSpace(id))
            {
                Logger.LogError($"{nameof(id)} is missing.");
                return BadRequest();
            }

            try
            {
                var request = new DeleteLineItemRequest(contextId, id);
                return await OnDeleteLineItemAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get the lineitems or a single lineitem for a context.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId, string id = null, 
            [FromQuery(Name = "lti_link_id")] string ltiLinkId = null, [FromQuery(Name = "resource_id")] string resourceId = null, 
            [FromQuery] string tag = null, [FromQuery] int? limit = null)
        {
            Logger.LogInformation(string.IsNullOrWhiteSpace(id)
                ? "Processing get lineitems request."
                : "Processing get lineitem request.");

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var request = new GetLineItemsRequest(contextId, ltiLinkId, resourceId, tag, limit);
                    return await OnGetLineItemsAsync(request).ConfigureAwait(false);
                }
                else
                {
                    var request = new GetLineItemRequest(contextId, id);
                    return await OnGetLineItemAsync(request).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, string.IsNullOrWhiteSpace(id) 
                    ? "Error processing get lineitems request." 
                    : "Error processing get lineitem request.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Create a new LineItem instance.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostAsync(string contextId, LineItem lineItem)
        {
            Logger.LogInformation("Processing post lineitem request.");

            if (!ModelState.IsValid)
            {
                Logger.LogError($"{nameof(lineItem)} model binding failed.");
                return BadRequest();
            }

            try
            {
                var request = new PostLineItemRequest(contextId, lineItem);
                return await OnCreateLineItemAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Update a particular LineItem instance.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> PutAsync(LineItem lineItem)
        {
            Logger.LogInformation("Processing put lineitem request.");

            if (!ModelState.IsValid)
            {
                Logger.LogError($"{nameof(lineItem)} model binding failed.");
                return BadRequest();
            }

            try
            {
                var request = new PutLineItemRequest(lineItem);
                return await OnUpdateLineItemAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Creates an <see cref="LineItemResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <param name="value">The LineItem to format in the entity body.</param>
        /// <returns>The created <see cref="LineItemResult"/> for the response.</returns>
        public LineItemResult Ok(LineItem value)
            => new LineItemResult(value);

        /// <summary>
        /// Creates an <see cref="LineItemContainerResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <param name="value">The LineItem to format in the entity body.</param>
        /// <returns>The created <see cref="LineItemContainerResult"/> for the response.</returns>
        public LineItemContainerResult Ok(LineItemContainer value)
            => new LineItemContainerResult(value);

        public new LineItemContainerResult NotFound()
            => new LineItemContainerResult(StatusCodes.Status404NotFound);

        /// <summary>
        /// Creates an <see cref="LineItemResult"/> object that produces an <see cref="StatusCodes.Status201Created"/> response.
        /// </summary>
        /// <param name="value">The LineItem to format in the entity body.</param>
        /// <returns>The created <see cref="LineItemResult"/> for the response.</returns>
        public LineItemResult Created(LineItem value)
        => new LineItemResult(StatusCodes.Status201Created)
        {
            Value = value
        };
    }
}