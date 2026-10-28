using System;
using System.Collections.Generic;
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
        // The spec requires its own media type for both directions (Proctoring 1.0 §5); plain JSON is
        // also accepted from callers that do not send it.
        [Consumes(Constants.MediaTypes.AssessmentControl, "application/json")]
        [Produces(Constants.MediaTypes.AssessmentControl)]
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
                    // An action this library does not recognise cannot be handed to the override
                    // as "Unknown"; tell the tool instead.
                    if (control.Action == AssessmentControlAction.Unknown)
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Unsupported action",
                            Status = StatusCodes.Status400BadRequest,
                            Detail = "The action is missing or not one this platform supports."
                        });
                    }

                    var missing = MissingRequiredFields(control);
                    if (missing.Count > 0)
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Invalid assessment control request",
                            Status = StatusCodes.Status400BadRequest,
                            Detail = string.Join(" ", missing)
                        });
                    }

                    var request = new ControlAssessmentRequest(control);
                    var result = await OnControlAssessmentAsync(request).ConfigureAwait(false);

                    // status is required in the response; never send it as null.
                    var response = result.Value ?? (result.Result as ObjectResult)?.Value as AssessmentControlResponse;
                    if (response != null && response.Status == AssessmentControlStatus.Unknown)
                        throw new InvalidOperationException(
                            $"{nameof(OnControlAssessmentAsync)} must set the response Status " +
                            $"(use {nameof(AssessmentControlStatus)}.{nameof(AssessmentControlStatus.None)} for no attempt in progress).");

                    return result;
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
                            : null // Exception messages can reveal configuration; Development only.
                    });
                }
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(ControlAssessmentAsync)}.");
            }
        }

        /// <summary>
        /// The Proctoring 1.0 §5.1 required request parameters that are missing, and an
        /// out-of-range <c>incident_severity</c>, as one message each.
        /// </summary>
        private static List<string> MissingRequiredFields(AssessmentControlRequest control)
        {
            var problems = new List<string>();
            if (string.IsNullOrWhiteSpace(control.User?.Iss)) problems.Add("user.iss is required.");
            if (string.IsNullOrWhiteSpace(control.User?.Sub)) problems.Add("user.sub is required.");
            if (string.IsNullOrWhiteSpace(control.ResourceLink?.Id)) problems.Add("resource_link.id is required.");
            if (control.AttemptNumber == null) problems.Add("attempt_number is required.");
            if (control.IncidentTime == null) problems.Add("incident_time is required.");
            if (control.IncidentSeverity is < 0 or > 1) problems.Add("incident_severity must be between 0 and 1.");
            if (control.ExtraTime is < 0) problems.Add("extra_time must not be negative.");
            return problems;
        }
    }
}
