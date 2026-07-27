using System;

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
        /// Tolerance applied to the "exp" and "iat" claims. Defaults to 5 minutes, matching the
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
        /// Records used nonces so a replayed token is rejected. Optional.
        /// </summary>
        /// <remarks>
        /// When null the validator still requires a nonce to be present but cannot tell a first
        /// use from a replay. Supply a store in production; see <see cref="INonceStore"/>.
        /// </remarks>
        public INonceStore NonceStore { get; set; }

        /// <summary>
        /// Whether the "nonce" claim must be present. Defaults to true, as LTI 1.3 requires.
        /// </summary>
        public bool RequireNonce { get; set; } = true;

        /// <summary>
        /// Whether the LTI "version" claim must be exactly "1.3.0". Defaults to true.
        /// </summary>
        public bool RequireLtiVersion { get; set; } = true;
    }
}
