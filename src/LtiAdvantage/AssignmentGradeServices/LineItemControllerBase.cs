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
    [Route("context/{contextid}/lineitems/{id}.{format}")]
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
        protected abstract Task<LineItemResult> OnDeleteLineItemAsync(DeleteLineItemRequest request);

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
        protected abstract Task<LineItemResult> OnUpdateLineItemAsync(PutLineItemRequest request);

        /// <summary>
        /// Delete a particular LineItem instance.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string contextId, string id)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(DeleteAsync)}.");

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
            finally 
            {
                Logger.LogDebug($"Entering {nameof(DeleteAsync)}.");
            }
        }

        /// <summary>
        /// Get the lineitem.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(string contextId, string id)
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
                    var request = new GetLineItemRequest(contextId, id);
                    return await OnGetLineItemAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing get lineitem request.");
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
        public async Task<IActionResult> PostAsync(string contextId, LineItem lineItem)
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
                    var request = new PostLineItemRequest(contextId, lineItem);
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

        /// <summary>
        /// Update a particular LineItem instance.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> PutAsync(LineItem lineItem)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(PutAsync)}.");

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
            finally
            {
                Logger.LogDebug($"Exiting {nameof(PutAsync)}.");
            }
        }

        #region Convenience methods to return a properly formatted  IActionResult
        
        /// <summary>
        /// Creates a LineItemResult with 200 status code.
        /// </summary>
        /// <param name="lineItem">The LineItem.</param>
        /// <returns>The created <see cref="LineItemResult"/> for the response.</returns>
        public LineItemResult LineItemOk(LineItem lineItem = null)
            => new LineItemResult(lineItem);

        /// <summary>
        /// Creates an empty LineItemResult with 404 status code.
        /// </summary>
        /// <returns>The created <see cref="LineItemResult"/> for the response.</returns>
        public LineItemResult LineItemNotFound()
            => new LineItemResult(StatusCodes.Status404NotFound);

        /// <summary>
        /// Creates a LineItemResult with 201 status code.
        /// </summary>
        /// <param name="lineItem">The LineItem to format in the entity body.</param>
        /// <returns>The created <see cref="LineItemResult"/> for the response.</returns>
        public LineItemResult LineItemCreated(LineItem lineItem)
            => new LineItemResult(lineItem, StatusCodes.Status201Created);

        #endregion
    }
}
