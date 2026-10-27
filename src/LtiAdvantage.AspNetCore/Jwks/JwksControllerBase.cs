using System.Linq;
using System.Threading.Tasks;
using LtiAdvantage.Jwks;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
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
                foreach (var key in keys ?? Enumerable.Empty<JsonWebKey>())
                {
                    var publicKey = ToPublicKey(key);
                    if (publicKey == null)
                    {
                        _logger.LogWarning("Skipping JWKS key {Kid}: it is not an RSA or EC key.", key?.Kid);
                        continue;
                    }
                    set.Keys.Add(publicKey);
                }
                return set;
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(GetJwksAsync)}.");
            }
        }

        /// <summary>
        /// Returns a copy of <paramref name="key"/> with only its public parameters,
        /// or <c>null</c> when the key is not an RSA or EC key (for example a symmetric key).
        /// </summary>
        public static JsonWebKey ToPublicKey(JsonWebKey key)
        {
            if (key == null) return null;

            var publicKey = new JsonWebKey
            {
                Kty = key.Kty,
                Kid = key.Kid,
                Use = key.Use,
                Alg = key.Alg,
                X5t = key.X5t,
                X5tS256 = key.X5tS256,
                X5u = key.X5u
            };
            foreach (var op in key.KeyOps) publicKey.KeyOps.Add(op);
            foreach (var cert in key.X5c) publicKey.X5c.Add(cert);

            switch (key.Kty)
            {
                case JsonWebAlgorithmsKeyTypes.RSA:
                    publicKey.N = key.N;
                    publicKey.E = key.E;
                    return publicKey;
                case JsonWebAlgorithmsKeyTypes.EllipticCurve:
                    publicKey.Crv = key.Crv;
                    publicKey.X = key.X;
                    publicKey.Y = key.Y;
                    return publicKey;
                default:
                    return null;
            }
        }
    }
}
