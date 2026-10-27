using System;
using System.Collections.Generic;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// Describes the platform whose ID tokens an <see cref="LtiTokenValidator"/> accepts.
    /// </summary>
    /// <remarks>
    /// One instance describes one platform-tool registration. A tool registered with several
    /// platforms needs one validator per registration, because <see cref="Issuer"/> and
    /// <see cref="ClientId"/> together identify the registration whose keys are trusted.
    /// </remarks>
    public class LtiTokenValidationOptions
    {
        /// <summary>
        /// The platform's issuer identifier. Required. A token whose "iss" differs is rejected.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The client id the platform assigned this tool. Required. A token that does not list it
        /// in "aud" is rejected.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The platform's JWKS URL. Required.
        /// </summary>
        public string JwksUri { get; set; }

        /// <summary>
        /// Tolerance applied to the "exp", "nbf" and "iat" claims ("iat" may not be later than now
        /// plus this). Defaults to 5 minutes, matching the
        /// default in <c>Microsoft.IdentityModel</c>.
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// How long a fetched JWKS is served before it is refetched. Defaults to 24 hours.
        /// </summary>
        public TimeSpan JwksCacheDuration { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// The shortest interval between two fetches of the same JWKS URL. Defaults to 5 minutes.
        /// </summary>
        /// <remarks>
        /// An unknown key id triggers an early refresh so a rotated key is picked up promptly.
        /// This interval bounds that path so tokens carrying bogus key ids cannot be turned into
        /// a stream of requests against the platform.
        /// </remarks>
        public TimeSpan MinimumRefreshInterval { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Records used nonces so a replayed token is rejected.
        /// </summary>
        /// <remarks>
        /// LTI 1.3 requires each nonce to be accepted at most once, and only a store can tell a
        /// first use from a replay. Required while <see cref="RequireNonce"/> is true, unless
        /// <see cref="AllowNonceReuse"/> is set; see <see cref="INonceStore"/>.
        /// </remarks>
        public INonceStore NonceStore { get; set; }

        /// <summary>
        /// Accept tokens without replay detection when no <see cref="NonceStore"/> is set.
        /// Defaults to false, so a missing store fails when the validator is constructed rather
        /// than silently letting replayed tokens through.
        /// </summary>
        /// <remarks>
        /// Only set this when replays are prevented elsewhere (for example the OIDC state check
        /// already consumes the nonce), or in tests.
        /// </remarks>
        public bool AllowNonceReuse { get; set; }

        /// <summary>
        /// Whether the "nonce" claim must be present. Defaults to true, as LTI 1.3 requires.
        /// </summary>
        public bool RequireNonce { get; set; } = true;

        /// <summary>
        /// The deployment ids this tool has registered for this platform and client. Optional.
        /// </summary>
        /// <remarks>
        /// LTI 1.3 requires a tool to check that the token's <c>deployment_id</c> is one it knows
        /// (one client id can have several deployments). When set, a token with any other
        /// deployment id is rejected. When null, only the claim's presence is checked, and the
        /// caller must check the value itself.
        /// </remarks>
        public ICollection<string> DeploymentIds { get; set; }

        /// <summary>
        /// Whether the LTI "version" claim must be exactly "1.3.0". Defaults to true.
        /// </summary>
        public bool RequireLtiVersion { get; set; } = true;
    }
}
