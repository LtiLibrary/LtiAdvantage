using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Platform-side endpoint for LTI Dynamic Registration 1.0.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#registration-endpoint
    /// </summary>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class DynamicRegistrationControllerBase : ControllerBase, IDynamicRegistrationController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DynamicRegistrationControllerBase> _logger;

        /// <summary>Initializes the base.</summary>
        protected DynamicRegistrationControllerBase(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Persist the registration. Implementations MUST set <see cref="ToolConfiguration.ClientId"/>
        /// on <c>request.Tool</c> (or return a new instance) before returning.
        /// </summary>
        protected abstract Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request);

        /// <inheritdoc />
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ToolConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.DynamicRegistration.Scope)]
        [Route("lti/register", Name = "lti-dynamic-registration")]
        public async Task<ActionResult<ToolConfiguration>> RegisterAsync([Required] [FromBody] ToolConfiguration tool)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(RegisterAsync)}.");
                var result = await OnRegisterAsync(new RegisterToolRequest(tool)).ConfigureAwait(false);
                if (result.Result is ObjectResult o && o.StatusCode == null) o.StatusCode = StatusCodes.Status201Created;
                else if (result.Value != null && result.Result == null) return Created(string.Empty, result.Value);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in {nameof(RegisterAsync)}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = _env.IsDevelopment() ? ex.Message + ex.StackTrace : ex.Message
                });
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(RegisterAsync)}.");
            }
        }
    }
}
