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
        /// Returns the public keys to publish in the JWKS document.
        ///
        /// Rotation guidance:
        /// - Always include the current signing key (the one used to sign tokens issued now).
        /// - Continue to include each retired signing key for at least the longest TTL of any token signed with it,
        ///   so verifiers can still validate in-flight tokens after rotation.
        /// - Each key MUST have a stable, unique <c>Kid</c>.
        /// - Set <c>Use = "sig"</c> and <c>Alg = "RS256"</c> for LTI 1.3 signing keys.
        /// - Do NOT include private key material — only the public components (<c>N</c>, <c>E</c>).
        /// </summary>
        Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync();
    }
}
