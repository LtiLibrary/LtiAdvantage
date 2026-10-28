using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LtiAdvantage.Jwks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>
    /// Publishes the platform's (or tool's) JWKS at a well-known URL so that
    /// peers can verify signed JWTs. Anonymous; unauthenticated.
    /// </summary>
    /// <remarks>
    /// Only public key parameters are published. Whatever the <see cref="IJwksKeyStore"/> returns,
    /// private parameters (<c>d</c>, <c>p</c>, <c>q</c>, <c>dp</c>, <c>dq</c>, <c>qi</c>) are never
    /// copied, and symmetric (<c>oct</c>) keys are skipped.
    /// </remarks>
    [ApiController]
    public abstract class JwksControllerBase : ControllerBase, IJwksController
    {
        private readonly IJwksKeyStore _keyStore;
        private readonly ILogger<JwksControllerBase> _logger;

        /// <summary>
        /// Constructs a new <see cref="JwksControllerBase"/>.
        /// </summary>
        /// <param name="keyStore">
        /// The key store providing public keys to publish. May be null when
        /// <see cref="GetPublicKeysAsync"/> is overridden.
        /// </param>
        /// <param name="logger">The logger.</param>
        protected JwksControllerBase(IJwksKeyStore keyStore, ILogger<JwksControllerBase> logger)
        {
            _keyStore = keyStore;
            _logger = logger;
        }

        /// <summary>
        /// Constructs a new <see cref="JwksControllerBase"/> without a key store, for a controller
        /// that overrides <see cref="GetPublicKeysAsync"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected JwksControllerBase(ILogger<JwksControllerBase> logger) : this(null, logger)
        {
        }

        /// <summary>
        /// Returns the JSON Web Key Set containing the public keys used to verify
        /// signed JWTs issued by this server.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Produces(Constants.MediaTypes.Jwks)]
        [ProducesResponseType(typeof(JwksDocument), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route(".well-known/jwks.json", Name = Constants.ServiceEndpoints.Jwks.JwksService)]
        public async Task<ActionResult<JwksDocument>> GetJwksAsync()
        {
            _logger.LogDebug($"Entering {nameof(GetJwksAsync)}.");
            try
            {
                var keys = await GetPublicKeysAsync().ConfigureAwait(false);

                var maxAge = (long)CacheDuration.TotalSeconds;
                Response.Headers[HeaderNames.CacheControl] = maxAge > 0 ? $"public, max-age={maxAge}" : "no-store";

                var set = new JwksDocument();
                foreach (var key in keys ?? Enumerable.Empty<JsonWebKey>())
                {
                    var publicKey = ToPublicKey(key);
                    if (publicKey == null)
                    {
                        _logger.LogWarning(
                            "Skipping JWKS key {Kid}: it is not an RSA or EC key with all its public parameters, or none of its key_ops has a public counterpart.", key?.Kid);
                        continue;
                    }
                    set.Keys.Add(PublishedJsonWebKey.From(publicKey));
                }
                return set;
            }
            catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                // The caller went away; let the framework abort the response.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in {nameof(GetJwksAsync)}.");
                Response.Headers[HeaderNames.CacheControl] = "no-store";
                // This endpoint is public, so no exception details are returned.
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "The key set could not be loaded",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetJwksAsync)}.");
            }
        }

        /// <summary>
        /// Returns the keys to publish. By default, the injected <see cref="IJwksKeyStore"/>'s keys.
        /// </summary>
        /// <remarks>
        /// Override this to publish a different key set per request, for example one per platform
        /// registration. Move the route so it carries the registration, then read it here:
        /// <code>
        /// // Startup: services.AddControllers(o => o.MapLtiRoute(
        /// //     Constants.ServiceEndpoints.Jwks.JwksService, "registrations/{registrationId}/jwks.json"));
        /// protected override Task&lt;IReadOnlyList&lt;JsonWebKey&gt;&gt; GetPublicKeysAsync() =>
        ///     _registrations.GetPublicKeysAsync((string) RouteData.Values["registrationId"], HttpContext.RequestAborted);
        /// </code>
        /// Private parameters are still removed from whatever this returns.
        /// </remarks>
        protected virtual Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync() =>
            (_keyStore ?? throw new InvalidOperationException(
                $"Pass an {nameof(IJwksKeyStore)} to the constructor or override {nameof(GetPublicKeysAsync)}."))
            .GetPublicKeysAsync();

        /// <summary>
        /// How long clients and proxies may cache the JWKS document. Default 5 minutes.
        /// Keep it well below the overlap during which a retired key stays published (see
        /// <see cref="IJwksKeyStore"/>), so verifiers pick up a new signing key in time.
        /// Return <see cref="TimeSpan.Zero"/> to disable caching.
        /// </summary>
        protected virtual TimeSpan CacheDuration => TimeSpan.FromMinutes(5);

        /// <summary>
        /// Returns a copy of <paramref name="key"/> with only its public parameters,
        /// or <c>null</c> when the key is not an RSA or EC key (for example a symmetric key), or is
        /// missing public parameters (<c>n</c>/<c>e</c> or <c>crv</c>/<c>x</c>/<c>y</c>) that verifiers need,
        /// or is restricted by <c>key_ops</c> to operations that have no public counterpart.
        /// See <see cref="PublicJsonWebKey.From"/>.
        /// </summary>
        public static JsonWebKey ToPublicKey(JsonWebKey key) => PublicJsonWebKey.From(key);
    }
}
