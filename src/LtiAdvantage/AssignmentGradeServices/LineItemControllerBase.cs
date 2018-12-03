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
    [Route("context/{contextid}/lineitems/{id}", Name = Constants.ServiceEndpoints.AgsLineItemService)]
    public abstract class LineItemControllerBase : Controller
    {
        protected readonly ILogger<LineItemsControllerBase> Logger;

        protected LineItemControllerBase(ILogger<LineItemsControllerBase> logger)
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
        /// Get the lineitem.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId, string id = null)
        {
            Logger.LogInformation("Processing get lineitem request.");

            try
            {
                var request = new GetLineItemRequest(contextId, id);
                return await OnGetLineItemAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing get lineitem request.");
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

        public LineItemResult NotFound(LineItem lineItem)
            => new LineItemResult(StatusCodes.Status404NotFound);

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
