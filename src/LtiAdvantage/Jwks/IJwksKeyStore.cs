using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.Jwks
{
    /// <summary>
    /// Provides the set of public keys to publish at /.well-known/jwks.json.
    /// Implementations decide how keys are stored, rotated, and retired —
    /// typically all currently-active and recently-retired (still in token TTL)
    /// signing keys are returned, each with a stable <c>kid</c>.
    /// </summary>
    public interface IJwksKeyStore
    {
        /// <summary>
        /// Returns the public keys that should appear in the JWKS document.
        /// Each key MUST have <c>Kid</c> set; <c>Use</c> SHOULD be "sig"; <c>Alg</c> SHOULD be "RS256".
        /// </summary>
        Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync();
    }
}
