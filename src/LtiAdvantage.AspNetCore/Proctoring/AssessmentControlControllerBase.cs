using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.Proctoring;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.Proctoring
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the platform side of the Assessment Control Service, which lets a proctoring
    /// tool pause, resume, terminate, update or flag an assessment attempt in progress.
    /// See https://www.imsglobal.org/spec/proctoring/v1p0#assessment-control-service.
    /// </summary>
    /// <remarks>
    /// The URL of this endpoint is what the platform advertises as
    /// <see cref="AssessmentControlClaimValueType.AssessmentControlUrl"/> on the
    /// LtiStartProctoring message.
    /// </remarks>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class AssessmentControlControllerBase : ControllerBase, IAssessmentControlController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AssessmentControlControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected AssessmentControlControllerBase(IWebHostEnvironment env,
            ILogger<AssessmentControlControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Applies the control action and returns the resulting state of the attempt.
        /// </summary>
        protected abstract Task<ActionResult<AssessmentControlResponse>> OnControlAssessmentAsync(
            ControlAssessmentRequest request);

        /// <summary>
        /// Applies a control action to an assessment attempt.
        /// </summary>
        /// <param name="control">The control action to apply.</param>
        /// <returns>The state of the attempt after the action was applied.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AssessmentControlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Ap.ControlAll)]
        [Route("assessment-control", Name = Constants.ServiceEndpoints.Ap.AssessmentControlService)]
        [Route("assessment-control.{format}")]
        public async Task<ActionResult<AssessmentControlResponse>> ControlAssessmentAsync(
            [Required] [FromBody] AssessmentControlRequest control)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(ControlAssessmentAsync)}.");

                try
                {
                    var request = new ControlAssessmentRequest(control);
                    return await OnControlAssessmentAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // The caller went away. Let the framework abort the response rather than
                    // reporting a server error the caller will never see.
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {nameof(ControlAssessmentAsync)}.");
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
                _logger.LogDebug($"Exiting {nameof(ControlAssessmentAsync)}.");
            }
        }
    }
}
