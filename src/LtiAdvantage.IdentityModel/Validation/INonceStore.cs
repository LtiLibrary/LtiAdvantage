using System.Threading;
using System.Threading.Tasks;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// Records the nonces already seen so a replayed ID token can be rejected.
    /// </summary>
    /// <remarks>
    /// LTI 1.3 requires that a nonce is accepted at most once. A validator cannot enforce that
    /// on its own because it does not know what other instances of the tool have seen, so supply
    /// an implementation backed by whatever store the deployment shares (a distributed cache, a
    /// table, and so on). Entries only need to outlive the token lifetime plus
    /// <see cref="LtiTokenValidationOptions.ClockSkew"/>.
    /// </remarks>
    public interface INonceStore
    {
        /// <summary>
        /// Atomically records the nonce and reports whether it had already been used.
        /// </summary>
        /// <param name="nonce">The nonce from the ID token.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// True if the nonce had not been seen before and is now recorded; false if it was
        /// already used and the token must be rejected.
        /// </returns>
        /// <remarks>
        /// Implementations must make the check and the record a single atomic operation.
        /// A read followed by a separate write lets two concurrent replays both succeed.
        /// </remarks>
        Task<bool> TryConsumeAsync(string nonce, CancellationToken cancellationToken = default);
    }
}
