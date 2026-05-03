using System.Threading.Tasks;
using LtiAdvantage.Jwks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>
    /// Publishes the platform's (or tool's) JWKS at a well-known URL so that
    /// peers can verify signed JWTs. Anonymous; unauthenticated.
    /// </summary>
    [ApiController]
    public abstract class JwksControllerBase : ControllerBase, IJwksController
    {
        private readonly IJwksKeyStore _keyStore;
        private readonly ILogger<JwksControllerBase> _logger;

        /// <summary>
        /// Constructs a new <see cref="JwksControllerBase"/>.
        /// </summary>
        /// <param name="keyStore">The key store providing public keys to publish.</param>
        /// <param name="logger">The logger.</param>
        protected JwksControllerBase(IJwksKeyStore keyStore, ILogger<JwksControllerBase> logger)
        {
            _keyStore = keyStore;
            _logger = logger;
        }

        /// <summary>
        /// Returns the JSON Web Key Set containing the public keys used to verify
        /// signed JWTs issued by this server.
        /// </summary>
        [HttpGet]
        [Produces(Constants.MediaTypes.Jwks)]
        [ProducesResponseType(typeof(JsonWebKeySet), StatusCodes.Status200OK)]
        [Route(".well-known/jwks.json", Name = Constants.ServiceEndpoints.Jwks.JwksService)]
        public async Task<ActionResult<JsonWebKeySet>> GetJwksAsync()
        {
            _logger.LogDebug($"Entering {nameof(GetJwksAsync)}.");
            try
            {
                var keys = await _keyStore.GetPublicKeysAsync().ConfigureAwait(false);
                var set = new JsonWebKeySet();
                foreach (var k in keys) set.Keys.Add(k);
                return set;
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetJwksAsync)}.");
            }
        }
    }
}
