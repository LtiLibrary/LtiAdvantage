using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
using AuthenticationHeaderValue = System.Net.Http.Headers.AuthenticationHeaderValue;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Platform-side endpoint for LTI Dynamic Registration 1.0.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#registration-endpoint
    /// </summary>
    /// <remarks>
    /// A request goes through three steps, in this order:
    /// <list type="number">
    /// <item><see cref="AuthorizeRegistrationAsync"/> decides whether the caller may register (401/403 otherwise).</item>
    /// <item>The body is read and checked by <see cref="ValidateRegistration"/> (400 <c>invalid_client_metadata</c> otherwise).</item>
    /// <item><see cref="OnRegisterAsync"/> stores the registration and returns it with a new <c>client_id</c> (201).</item>
    /// </list>
    /// </remarks>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class DynamicRegistrationControllerBase : ControllerBase, IDynamicRegistrationController
    {
        private static readonly JsonSerializerOptions ResponseJsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DynamicRegistrationControllerBase> _logger;

        /// <summary>Initializes the base.</summary>
        protected DynamicRegistrationControllerBase(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Stores the registration and returns the registered tool configuration with a newly
        /// assigned <see cref="ToolConfiguration.ClientId"/>. The endpoint answers 201 Created with it.
        /// </summary>
        /// <remarks>
        /// Called only after <see cref="AuthorizeRegistrationAsync"/> allowed the request and
        /// <see cref="ValidateRegistration"/> found no errors. Any <c>client_id</c> the tool sent has
        /// already been cleared, so the platform always chooses it.
        /// <list type="bullet">
        /// <item>To reject the registration, throw <see cref="RegistrationRejectedException"/>. It can also be
        /// thrown from <see cref="AuthorizeRegistrationAsync"/> and <see cref="ValidateRegistration"/>.</item>
        /// <item>Returning <c>null</c>, or a configuration without a <c>client_id</c>, is a server error (500).</item>
        /// </list>
        /// </remarks>
        protected abstract Task<ToolConfiguration> OnRegisterAsync(RegisterToolRequest request);

        /// <summary>
        /// Decides whether a registration request may proceed. Return <c>null</c> to allow it,
        /// or a result (such as <c>Unauthorized()</c>) to reject it.
        /// </summary>
        /// <remarks>
        /// Runs before the request body is read. The spec leaves the <c>registration_token</c> optional
        /// and opaque (DR §3.3), so platforms that issue their own tokens (for example single-use tokens
        /// stored in a database), or accept registrations without one, should override this method.
        /// The default authenticates the request with the <see cref="JwtBearerDefaults.AuthenticationScheme"/>
        /// scheme and requires the <see cref="Constants.LtiScopes.DynamicRegistration.Scope"/> policy
        /// (registered by <c>AddLtiAdvantagePolicies</c>). If the scheme is not registered, every request
        /// is answered 401; if the policy is not registered, 403. Both log a warning.
        /// </remarks>
        /// <param name="registrationToken">
        /// The Bearer token from the <c>Authorization</c> header, or <c>null</c> when none was sent.
        /// </param>
        protected virtual async Task<ActionResult> AuthorizeRegistrationAsync(string registrationToken)
        {
            var schemes = HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            if (await schemes.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme).ConfigureAwait(false) == null)
            {
                _logger.LogWarning(
                    "Dynamic registration rejected: no '{Scheme}' authentication scheme is registered. " +
                    "Register one or override {Method}.",
                    JwtBearerDefaults.AuthenticationScheme, nameof(AuthorizeRegistrationAsync));
                return BearerUnauthorized(registrationToken);
            }

            var policies = HttpContext.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = await policies.GetPolicyAsync(Constants.LtiScopes.DynamicRegistration.Scope).ConfigureAwait(false);
            if (policy == null)
            {
                _logger.LogWarning(
                    "Dynamic registration rejected: no '{Policy}' authorization policy is registered. " +
                    "Call AddLtiAdvantagePolicies or override {Method}.",
                    Constants.LtiScopes.DynamicRegistration.Scope, nameof(AuthorizeRegistrationAsync));
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var authentication = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)
                .ConfigureAwait(false);
            if (!authentication.Succeeded) return BearerUnauthorized(registrationToken);

            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorization = await authorizationService.AuthorizeAsync(authentication.Principal, policy)
                .ConfigureAwait(false);
            return authorization.Succeeded ? null : StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// A 401 with the <c>WWW-Authenticate: Bearer</c> challenge required by RFC 6750 §3. When a token
        /// was sent it adds <c>error="invalid_token"</c>; when none was sent it adds no error code (§3.1).
        /// </summary>
        protected ActionResult BearerUnauthorized(string registrationToken)
        {
            Response.Headers[HeaderNames.WWWAuthenticate] = registrationToken == null
                ? "Bearer"
                : "Bearer error=\"invalid_token\"";
            return Unauthorized();
        }

        /// <summary>
        /// Checks the registration request for the metadata required by LTI Dynamic Registration 1.0
        /// (§2.2 and §3.1). Returns one message per problem; an empty list (or <c>null</c>) means the request is valid.
        /// Override to add platform-specific rules (call the base method to keep the standard checks).
        /// </summary>
        protected virtual IList<string> ValidateRegistration(ToolConfiguration tool)
        {
            var errors = new List<string>();

            if (tool.ApplicationType != "web")
                errors.Add("application_type must be web.");
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

            OptionalWebUrl(errors, "client_uri", tool.ClientUri);
            OptionalWebUrl(errors, "logo_uri", tool.LogoUri);

            var lti = tool.LtiToolConfiguration;
            if (lti == null)
            {
                errors.Add("https://purl.imsglobal.org/spec/lti-tool-configuration is required.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(lti.Domain)) errors.Add("domain is required.");
                else if (!IsBareHost(lti.Domain)) errors.Add("domain must be a host name (optionally with a port), without scheme or path.");
                foreach (var secondary in lti.SecondaryDomains ?? Enumerable.Empty<string>())
                    if (!IsBareHost(secondary))
                        errors.Add("secondary_domains must be host names (optionally with a port), without scheme or path.");
                RequireWebUrl(errors, "target_link_uri", lti.TargetLinkUri);

                foreach (var message in lti.Messages ?? Enumerable.Empty<MessageDescriptor>())
                {
                    if (message == null) continue;
                    if (string.IsNullOrWhiteSpace(message.Type)) errors.Add("messages.type is required.");
                    OptionalWebUrl(errors, "messages.target_link_uri", message.TargetLinkUri);
                    OptionalWebUrl(errors, "messages.icon_uri", message.IconUri);
                }
            }

            return errors;
        }

        /// <inheritdoc />
        /// <remarks>
        /// The body is read by the action itself (not model binding or a <c>[Consumes]</c> filter), so that
        /// authorization runs before anything about the body is reported back to the caller. The body is
        /// parsed as JSON whatever its <c>Content-Type</c>.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous] // Authorization is done by AuthorizeRegistrationAsync, since the token is optional.
        [Produces("application/json")]
        [ProducesResponseType(typeof(ToolConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegistrationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route("lti/register", Name = "lti-dynamic-registration")]
        public async Task<ActionResult<ToolConfiguration>> RegisterAsync()
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(RegisterAsync)}.");

                var rejected = await AuthorizeRegistrationAsync(GetBearerToken()).ConfigureAwait(false);
                if (rejected != null) return rejected;

                var (tool, readError) = await ReadToolConfigurationAsync().ConfigureAwait(false);
                if (tool == null)
                    return BadRequest(RegistrationError.InvalidClientMetadata(readError));

                var errors = ValidateRegistration(tool);
                if (errors != null && errors.Count > 0)
                {
                    // §3.6.2: reject with 400 and a JSON error body (RFC 7591 error format).
                    return BadRequest(RegistrationError.InvalidClientMetadata(string.Join(" ", errors)));
                }

                // The platform assigns the client_id; never keep one chosen by the tool.
                tool.ClientId = null;

                var registered = await OnRegisterAsync(new RegisterToolRequest(tool)).ConfigureAwait(false);

                if (registered == null || string.IsNullOrWhiteSpace(registered.ClientId))
                    throw new InvalidOperationException(
                        $"{nameof(OnRegisterAsync)} must return the registered tool configuration with a client_id.");

                // Leave unset fields out instead of writing explicit nulls, which strict tools reject.
                return new JsonResult(registered, ResponseJsonOptions) { StatusCode = StatusCodes.Status201Created };
            }
            catch (RegistrationRejectedException ex)
            {
                // Thrown by any step: AuthorizeRegistrationAsync, ValidateRegistration or OnRegisterAsync.
                return StatusCode(ex.StatusCode, new RegistrationError { Error = ex.Error, ErrorDescription = ex.Description });
            }
            catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                // The client disconnected; there is no one to answer and nothing to log as an error.
                _logger.LogDebug($"{nameof(RegisterAsync)} was cancelled because the client disconnected.");
                return new EmptyResult();
            }
            catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                // The caller went away. Let the framework abort the response rather than
                // reporting a server error the caller will never see.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in {nameof(RegisterAsync)}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    // Exception details can reveal configuration, so they are shown only in development.
                    Detail = _env.IsDevelopment() ? ex.Message + ex.StackTrace : null
                });
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(RegisterAsync)}.");
            }
        }

        private async Task<(ToolConfiguration Tool, string Error)> ReadToolConfigurationAsync()
        {
            const string expected = "The request body must be a JSON tool configuration.";
            try
            {
                var tool = await JsonSerializer
                    .DeserializeAsync<ToolConfiguration>(Request.Body, cancellationToken: HttpContext.RequestAborted)
                    .ConfigureAwait(false);
                return (tool, tool == null ? expected : null);
            }
            catch (JsonException ex)
            {
                // Name the offending field when the JSON is valid but a value has the wrong type.
                var path = string.IsNullOrEmpty(ex.Path) || ex.Path == "$" ? null : ex.Path;
                return (null, path == null ? expected : $"{expected} The value at {path} is invalid.");
            }
        }

        private string GetBearerToken()
        {
            var headers = Request.Headers[HeaderNames.Authorization];
            if (headers.Count != 1) return null; // Missing, or ambiguous with several values.

            return AuthenticationHeaderValue.TryParse(headers[0], out var header)
                && string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(header.Parameter)
                    ? header.Parameter.Trim()
                    : null;
        }

        /// <summary>Requires a URL accepted by <see cref="RegistrationUrl.IsAllowed(string)"/>.</summary>
        private static void RequireWebUrl(ICollection<string> errors, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{name} is required.");
            else if (!RegistrationUrl.IsAllowed(value))
                errors.Add($"{name} must be an absolute https URL.");
        }

        /// <summary>Like <see cref="RequireWebUrl"/>, but the value may be absent.</summary>
        private static void OptionalWebUrl(ICollection<string> errors, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !RegistrationUrl.IsAllowed(value))
                errors.Add($"{name} must be an absolute https URL.");
        }

        /// <summary>True for <c>tool.example.com</c> or <c>tool.example.com:8443</c>; false for URLs or paths.</summary>
        private static bool IsBareHost(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.IndexOfAny(new[] { '/', '?', '#', '@', ' ' }) >= 0) return false;
            return Uri.TryCreate("https://" + value, UriKind.Absolute, out var uri)
                && string.Equals(uri.Authority, value, StringComparison.OrdinalIgnoreCase)
                && Uri.CheckHostName(uri.Host) != UriHostNameType.Unknown;
        }

        private static void RequireValues(ICollection<string> errors, string name, IList<string> actual, params string[] required)
        {
            var missing = required.Where(r => actual == null || !actual.Contains(r)).ToList();
            if (missing.Count > 0)
                errors.Add($"{name} must include {string.Join(" and ", missing)}.");
        }
    }
}
