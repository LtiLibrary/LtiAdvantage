using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

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
        /// Called only after <see cref="AuthorizeRegistrationAsync"/> allowed the request and
        /// <see cref="ValidateRegistration"/> found no errors.
        /// <list type="bullet">
        /// <item>To accept, return the registered <see cref="ToolConfiguration"/>, as a value or inside a
        /// 2xx (or status-less) <see cref="ObjectResult"/> such as <c>Ok(tool)</c>. The response is always
        /// 201 Created, and a missing <c>client_id</c> is a server error.</item>
        /// <item>To reject, return an error result such as <c>BadRequest(new RegistrationError { ... })</c>.
        /// It is passed through; an <see cref="ObjectResult"/> without a status code is sent as 400.</item>
        /// </list>
        /// </remarks>
        protected abstract Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request);

        /// <summary>
        /// Decides whether a registration request may proceed. Return <c>null</c> to allow it,
        /// or a result (such as <c>Unauthorized()</c> or <c>Forbid()</c>) to reject it.
        /// </summary>
        /// <remarks>
        /// The spec leaves the <c>registration_token</c> optional and opaque (DR §3.3), so platforms that
        /// issue their own tokens (for example single-use tokens stored in a database), or accept
        /// registrations without one, should override this method.
        /// The default authenticates the request with the <see cref="JwtBearerDefaults.AuthenticationScheme"/>
        /// scheme and requires the <see cref="Constants.LtiScopes.DynamicRegistration.Scope"/> policy
        /// (registered by <c>AddLtiAdvantagePolicies</c>).
        /// </remarks>
        /// <param name="registrationToken">
        /// The Bearer token from the <c>Authorization</c> header, or <c>null</c> when none was sent.
        /// </param>
        protected virtual async Task<ActionResult> AuthorizeRegistrationAsync(string registrationToken)
        {
            var authentication = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)
                .ConfigureAwait(false);
            if (!authentication.Succeeded) return Unauthorized();

            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorization = await authorizationService
                .AuthorizeAsync(authentication.Principal, Constants.LtiScopes.DynamicRegistration.Scope)
                .ConfigureAwait(false);
            return authorization.Succeeded ? null : StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Checks the registration request for the metadata required by LTI Dynamic Registration 1.0
        /// (§2.2 and §3.1). Returns one message per problem; an empty list means the request is valid.
        /// Override to add platform-specific rules (call the base method to keep the standard checks).
        /// </summary>
        protected virtual IList<string> ValidateRegistration(ToolConfiguration tool)
        {
            var errors = new List<string>();

            RequireWebUrl(errors, "initiate_login_uri", tool.InitiateLoginUri);
            RequireWebUrl(errors, "jwks_uri", tool.JwksUri);

            if (tool.RedirectUris == null || tool.RedirectUris.Count == 0)
                errors.Add("redirect_uris is required.");
            else
                foreach (var uri in tool.RedirectUris) RequireWebUrl(errors, "redirect_uris", uri);

            if (tool.TokenEndpointAuthMethod != "private_key_jwt")
                errors.Add("token_endpoint_auth_method must be private_key_jwt.");
            RequireValues(errors, "grant_types", tool.GrantTypes, "implicit", "client_credentials");
            RequireValues(errors, "response_types", tool.ResponseTypes, "id_token");

            var lti = tool.LtiToolConfiguration;
            if (lti == null)
            {
                errors.Add("https://purl.imsglobal.org/spec/lti-tool-configuration is required.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(lti.Domain)) errors.Add("domain is required.");
                RequireWebUrl(errors, "target_link_uri", lti.TargetLinkUri);
            }

            return errors;
        }

        /// <inheritdoc />
        [HttpPost]
        [AllowAnonymous] // Authorization is done by AuthorizeRegistrationAsync, since the token is optional.
        [RegistrationModelStateFilter]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ToolConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegistrationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("lti/register", Name = "lti-dynamic-registration")]
        public async Task<ActionResult<ToolConfiguration>> RegisterAsync([Required] [FromBody] ToolConfiguration tool)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(RegisterAsync)}.");

                var rejected = await AuthorizeRegistrationAsync(GetBearerToken()).ConfigureAwait(false);
                if (rejected != null) return rejected;

                var errors = ValidateRegistration(tool);
                if (errors.Count > 0)
                {
                    // §3.6.2: reject with 400 and a JSON error body (RFC 7591 error format).
                    return BadRequest(RegistrationError.InvalidClientMetadata(string.Join(" ", errors)));
                }

                var result = await OnRegisterAsync(new RegisterToolRequest(tool)).ConfigureAwait(false);
                var objectResult = result.Result as ObjectResult;
                var registered = result.Value ?? objectResult?.Value as ToolConfiguration;
                var isSuccess = result.Result == null
                    || (objectResult != null && (objectResult.StatusCode == null || IsSuccessStatusCode(objectResult.StatusCode.Value)));

                if (registered == null || !isSuccess)
                {
                    // A rejection. An ObjectResult with no status code would otherwise go out as 200 OK.
                    if (objectResult != null && objectResult.StatusCode == null)
                        objectResult.StatusCode = StatusCodes.Status400BadRequest;
                    return result;
                }

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

        private string GetBearerToken()
        {
            var header = Request.Headers[HeaderNames.Authorization].ToString();
            const string prefix = "Bearer ";
            if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
            var token = header.Substring(prefix.Length).Trim();
            return token.Length == 0 ? null : token;
        }

        private static bool IsSuccessStatusCode(int statusCode) => statusCode >= 200 && statusCode < 300;

        /// <summary>
        /// Requires an absolute https URL (http is allowed only for loopback hosts, for local development).
        /// Other schemes, such as <c>javascript:</c> or <c>file:</c>, are rejected. A bare path such as
        /// <c>/login</c> is rejected too: on Unix, <see cref="Uri.TryCreate(string, UriKind, out Uri)"/>
        /// would otherwise treat it as an absolute <c>file:</c> URI.
        /// </summary>
        private static void RequireWebUrl(ICollection<string> errors, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{name} is required.");
                return;
            }

            var isWebUrl = Uri.TryCreate(value, UriKind.Absolute, out var uri)
                && !uri.IsFile && !uri.IsUnc
                && (uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback));
            if (!isWebUrl)
                errors.Add($"{name} must be an absolute https URL.");
        }

        private static void RequireValues(ICollection<string> errors, string name, IList<string> actual, params string[] required)
        {
            var missing = required.Where(r => actual == null || !actual.Contains(r)).ToList();
            if (missing.Count > 0)
                errors.Add($"{name} must include {string.Join(" and ", missing)}.");
        }
    }
}
