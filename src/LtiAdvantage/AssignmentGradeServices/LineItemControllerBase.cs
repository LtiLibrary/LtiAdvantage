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
    [Route("context/{contextId}/lineitems/{id}", Name = Constants.ServiceEndpoints.AgsLineItemService)]
    [Route("context/{contextId}/lineitems/{id}.{format}")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public abstract class LineItemControllerBase : ControllerBase
    {
        /// <summary>
        /// </summary>
        protected readonly ILogger<LineItemsControllerBase> Logger;

        /// <summary>
        /// </summary>
        protected LineItemControllerBase(ILogger<LineItemsControllerBase> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Deletes a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<ActionResult> OnDeleteLineItemAsync(DeleteLineItemRequest request);

        /// <summary>
        /// Updates a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The result.</returns>
        protected abstract Task<ActionResult> OnUpdateLineItemAsync(UpdateLineItemRequest request);

        /// <summary>
        /// Deletes a line item.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="id">The line item id.</param>
        /// <returns>The result.</returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(string contextId, string id)
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
        /// Updates a line item.
        /// </summary>
        /// <param name="lineItem">The updated line item.</param>
        /// <returns>The result.</returns>
        [HttpPut]
        [Consumes(Constants.MediaTypes.LineItem)]
        public async Task<ActionResult> PutAsync(LineItem lineItem)
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
                    var request = new UpdateLineItemRequest(lineItem);
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
    }
}
