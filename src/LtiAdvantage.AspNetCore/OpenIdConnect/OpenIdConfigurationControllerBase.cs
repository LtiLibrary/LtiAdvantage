using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.OpenIdConnect
{
    /// <summary>
    /// Publishes the platform's OpenID Provider Metadata, including the LTI platform
    /// configuration claim a tool needs in order to register itself dynamically.
    /// Anonymous; unauthenticated.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#openid-configuration.
    /// </summary>
    /// <remarks>
    /// The counterpart to <see cref="Jwks.JwksControllerBase"/>: that one publishes the keys,
    /// this one publishes where to find them. Unlike the JWKS controller, the document is
    /// produced by an override rather than an injected store, because most of its values are
    /// absolute URLs to this same host and are therefore easiest to build from the current
    /// request — see <see cref="OnGetOpenIdConfigurationAsync"/>.
    /// <para>
    /// The route is <c>.well-known/openid-configuration</c>, which an OpenID Connect server in the
    /// same app (such as OpenIddict) also serves. Either turn that one off (OpenIddict:
    /// <c>SetConfigurationEndpointUris(Array.Empty&lt;string&gt;())</c>) or move this one with
    /// <c>MvcOptions.MapLtiRoute(Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService, "...")</c>.
    /// Tools find the document through the <c>openid_configuration</c> URL the platform sends when
    /// it starts a registration, so any path works.
    /// </para>
    /// </remarks>
    [ApiController]
    public abstract class OpenIdConfigurationControllerBase : ControllerBase, IOpenIdConfigurationController
    {
        private readonly ILogger<OpenIdConfigurationControllerBase> _logger;

        /// <summary>
        /// Constructs a new <see cref="OpenIdConfigurationControllerBase"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected OpenIdConfigurationControllerBase(ILogger<OpenIdConfigurationControllerBase> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the metadata document to publish.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Every URL in the document must be absolute. <c>Url.Link(routeName, null)</c> resolves
        /// the library's named routes against the current request, so
        /// <c>Url.Link(Constants.ServiceEndpoints.Jwks.JwksService, null)</c> yields this
        /// platform's JWKS URL without hard-coding a host.
        /// </para>
        /// <para>
        /// <see cref="PlatformOpenIdConfiguration.Issuer"/> must be the same issuer string the
        /// platform puts in the "iss" claim of its ID tokens; a tool that finds the two
        /// disagreeing has no way to tell which one identifies the platform.
        /// </para>
        /// </remarks>
        protected abstract Task<PlatformOpenIdConfiguration> OnGetOpenIdConfigurationAsync();

        /// <summary>
        /// Returns the platform's OpenID Provider Metadata document.
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Public metadata: tools fetch it before they hold any credentials.
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlatformOpenIdConfiguration), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route(".well-known/openid-configuration",
            Name = Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService)]
        public async Task<ActionResult<PlatformOpenIdConfiguration>> GetOpenIdConfigurationAsync()
        {
            _logger.LogDebug($"Entering {nameof(GetOpenIdConfigurationAsync)}.");
            try
            {
                var configuration = await OnGetOpenIdConfigurationAsync().ConfigureAwait(false);
                if (configuration == null)
                    throw new InvalidOperationException(
                        $"{nameof(OnGetOpenIdConfigurationAsync)} must return the platform's OpenID configuration.");
                return configuration;
            }
            catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                // The caller went away; let the framework abort the response.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in {nameof(GetOpenIdConfigurationAsync)}.");
                // This endpoint is public, so no exception details are returned.
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "The OpenID configuration could not be loaded",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetOpenIdConfigurationAsync)}.");
            }
        }
    }
}
