using System;
using System.Collections.Generic;
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
        /// <remarks>
        /// Called only after <see cref="ValidateRegistration"/> found no errors. Return the registered
        /// <see cref="ToolConfiguration"/> (as a value or <see cref="ObjectResult"/>) to answer 201 Created;
        /// return any other result, such as <c>BadRequest(...)</c>, to reject the registration.
        /// </remarks>
        protected abstract Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request);

        /// <summary>
        /// Checks the registration request for the metadata required by LTI Dynamic Registration 1.0 §2.2.
        /// Returns one message per problem; an empty list means the request is valid.
        /// Override to add platform-specific rules (call the base method to keep the standard checks).
        /// </summary>
        protected virtual IList<string> ValidateRegistration(ToolConfiguration tool)
        {
            var errors = new List<string>();

            RequireAbsoluteUrl(errors, "initiate_login_uri", tool.InitiateLoginUri);
            RequireAbsoluteUrl(errors, "jwks_uri", tool.JwksUri);

            if (tool.RedirectUris == null || tool.RedirectUris.Count == 0)
                errors.Add("redirect_uris is required.");
            else
                foreach (var uri in tool.RedirectUris) RequireAbsoluteUrl(errors, "redirect_uris", uri);

            var lti = tool.LtiToolConfiguration;
            if (lti == null)
            {
                errors.Add("https://purl.imsglobal.org/spec/lti-tool-configuration is required.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(lti.Domain)) errors.Add("domain is required.");
                RequireAbsoluteUrl(errors, "target_link_uri", lti.TargetLinkUri);
            }

            return errors;
        }

        private static void RequireAbsoluteUrl(ICollection<string> errors, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{name} is required.");
            else if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                errors.Add($"{name} must be an absolute URL.");
        }

        /// <inheritdoc />
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ToolConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegistrationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.DynamicRegistration.Scope)]
        [Route("lti/register", Name = "lti-dynamic-registration")]
        public async Task<ActionResult<ToolConfiguration>> RegisterAsync([Required] [FromBody] ToolConfiguration tool)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(RegisterAsync)}.");

                var errors = ValidateRegistration(tool);
                if (errors.Count > 0)
                {
                    // §3.6.2: reject with 400 and a JSON error body (RFC 7591 error format).
                    return BadRequest(new RegistrationError
                    {
                        Error = "invalid_client_metadata",
                        ErrorDescription = string.Join(" ", errors)
                    });
                }

                var result = await OnRegisterAsync(new RegisterToolRequest(tool)).ConfigureAwait(false);

                // A registered tool configuration (as a value or an ObjectResult without a status code)
                // becomes 201 Created. Every other result, such as an error the override chose, passes through.
                var registered = result.Value;
                if (registered == null && result.Result is ObjectResult o
                    && (o.StatusCode == null || o.StatusCode == StatusCodes.Status201Created))
                {
                    registered = o.Value as ToolConfiguration;
                }
                if (registered == null) return result;

                if (string.IsNullOrWhiteSpace(registered.ClientId))
                    throw new InvalidOperationException(
                        $"{nameof(OnRegisterAsync)} must assign a client_id to the registered tool configuration.");

                return StatusCode(StatusCodes.Status201Created, registered);
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
