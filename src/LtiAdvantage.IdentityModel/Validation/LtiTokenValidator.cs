using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// Validates an LTI 1.3 ID token end to end: signature against the platform's published
    /// JWKS, then the registered claims (iss, aud, exp) and the LTI claims (version,
    /// message_type, deployment_id, nonce).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The library has always been able to read an ID token's claims, but reading is not
    /// verifying. Use this class when the tool terminates the launch itself; if the launch
    /// already passes through JWT bearer middleware configured against the platform's JWKS,
    /// that middleware has done the signature and registered-claim work and only the LTI
    /// claim checks remain.
    /// </para>
    /// <para>
    /// One validator serves one platform-tool registration and is safe to share across
    /// requests, which is the point: the JWKS it fetches is cached rather than refetched per
    /// launch. Register it as a singleton.
    /// </para>
    /// </remarks>
    public class LtiTokenValidator
    {
        private readonly LtiTokenValidationOptions _options;
        private readonly JwksCache _jwksCache;

        // Shared: JwtSecurityTokenHandler is safe for concurrent ReadJwtToken/ValidateToken calls.
        private static readonly JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler
        {
            // Keep the claim types exactly as the platform sent them. The default inbound
            // mapping rewrites "sub" and friends to WS-* URIs, which would break every
            // lookup by LTI claim name.
            MapInboundClaims = false
        };

        /// <summary>
        /// Constructs a new <see cref="LtiTokenValidator"/>.
        /// </summary>
        /// <param name="options">The platform-tool registration to validate against.</param>
        /// <param name="client">
        /// The HTTP client used to fetch the platform's JWKS. Supply one from
        /// <c>IHttpClientFactory</c> so socket handling and DNS refresh are managed for you.
        /// </param>
        public LtiTokenValidator(LtiTokenValidationOptions options, HttpClient client)
            : this(options, CreateCache(options, client))
        {
        }

        /// <summary>
        /// Constructs a validator that uses a shared <see cref="JwksCache"/>.
        /// </summary>
        /// <remarks>
        /// Use this when several validators (for example one per client id or deployment) trust
        /// the same platform JWKS URL, so they share one set of fetches, refreshes and rate limits.
        /// The cache's own durations apply; the cache settings in <paramref name="options"/> are
        /// ignored.
        /// </remarks>
        /// <param name="options">The registration details and validation settings.</param>
        /// <param name="jwksCache">The shared key cache.</param>
        public LtiTokenValidator(LtiTokenValidationOptions options, JwksCache jwksCache)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _jwksCache = jwksCache ?? throw new ArgumentNullException(nameof(jwksCache));

            if (string.IsNullOrWhiteSpace(options.Issuer))
                throw new ArgumentException($"{nameof(options.Issuer)} is required", nameof(options));
            if (string.IsNullOrWhiteSpace(options.ClientId))
                throw new ArgumentException($"{nameof(options.ClientId)} is required", nameof(options));
            if (string.IsNullOrWhiteSpace(options.JwksUri))
                throw new ArgumentException($"{nameof(options.JwksUri)} is required", nameof(options));
            if (options.RequireNonce && options.NonceStore == null && !options.AllowNonceReuse)
                throw new ArgumentException(
                    $"{nameof(options.NonceStore)} is required to reject replayed tokens. Supply one, or set " +
                    $"{nameof(options.AllowNonceReuse)} to accept tokens without replay detection.", nameof(options));

        }

        private static JwksCache CreateCache(LtiTokenValidationOptions options, HttpClient client)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (client == null) throw new ArgumentNullException(nameof(client));
            return new JwksCache(client, options.JwksCacheDuration, options.MinimumRefreshInterval,
                options.MaximumKeyStaleness);
        }

        /// <summary>
        /// Validates an ID token.
        /// </summary>
        /// <param name="idToken">The serialized JWT from the launch.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// The outcome. Failures are reported on the result rather than thrown, because an
        /// invalid launch is an expected condition rather than a fault. That includes the case
        /// where the platform's JWKS cannot be fetched: the token's key id is chosen by whoever
        /// sent it, so a fetch failure must not surface as an exception. Only cancellation of
        /// <paramref name="cancellationToken"/> is thrown.
        /// </returns>
        public Task<LtiTokenValidationResult> ValidateAsync(string idToken,
            CancellationToken cancellationToken = default) =>
            ValidateAsync(idToken, null, cancellationToken);

        /// <summary>
        /// Validates an ID token and checks that its nonce is the one this browser's login asked for.
        /// </summary>
        /// <remarks>
        /// A tool sends a nonce with the OIDC login redirect and keeps it with the <c>state</c> (for
        /// example in a cookie). Pass that nonce here: a token whose nonce differs was not issued for
        /// this login, even if it is otherwise valid and unused, and is rejected before the
        /// <see cref="LtiTokenValidationOptions.NonceStore"/> records it.
        /// </remarks>
        /// <param name="idToken">The serialized JWT from the launch.</param>
        /// <param name="expectedNonce">
        /// The nonce bound to this browser's <c>state</c>. Null skips the check, as
        /// <see cref="ValidateAsync(string, CancellationToken)"/> does.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The outcome; see <see cref="ValidateAsync(string, CancellationToken)"/>.</returns>
        public async Task<LtiTokenValidationResult> ValidateAsync(string idToken, string expectedNonce,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(idToken))
            {
                return LtiTokenValidationResult.Failure("The id_token is missing.");
            }

            var handler = Handler;


            string keyId;
            try
            {
                // Also the readability check: a token that is not a JWT fails here.
                keyId = handler.ReadJwtToken(idToken).Header.Kid;
            }
            catch (Exception ex)
            {
                return LtiTokenValidationResult.Failure("The id_token is not a readable JWT.", ex);
            }

            IReadOnlyCollection<SecurityKey> keys;
            try
            {
                keys = await _jwksCache
                    .GetSigningKeysAsync(_options.JwksUri, keyId, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
            {
                // Fetch failures, timeouts and malformed JWKS documents.
                // Generic text: the details (JWKS URL, HTTP status) are configuration, and Error
                // may end up on a page. They stay available on Exception for logging.
                return LtiTokenValidationResult.Failure("The platform's signing keys could not be fetched.", ex);
            }

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.ClientId,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = _options.ClockSkew,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                IssuerSigningKeys = keys,

                // LTI 1.3 signs with RS256. Pinning the algorithm keeps a token from talking
                // the validator into a weaker one.
                ValidAlgorithms = new[] {SecurityAlgorithms.RsaSha256}
            };

            System.Security.Claims.ClaimsPrincipal principal;
            SecurityToken validatedToken;
            try
            {
                principal = handler.ValidateToken(idToken, parameters, out validatedToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
            {
                // Security token failures, and anything else signature checking can throw (for
                // example a crypto provider rejecting a platform key), are failed validations.
                return LtiTokenValidationResult.Failure($"The id_token failed validation: {ex.Message}", ex);
            }

            if (validatedToken is not JwtSecurityToken jwt)
            {
                return LtiTokenValidationResult.Failure("The id_token is not a JWS.");
            }

            try
            {
                var ltiFailure = ValidateLtiClaims(jwt);
                if (ltiFailure != null)
                {
                    return ltiFailure;
                }

                var nonceFailure = await ValidateNonceAsync(jwt, expectedNonce, cancellationToken).ConfigureAwait(false);
                if (nonceFailure != null)
                {
                    return nonceFailure;
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
            {
                // A malformed claim (such as a non-numeric iat) or a failing nonce store must still
                // come back as a failed validation, as this method promises.
                return LtiTokenValidationResult.Failure("The id_token could not be validated.", ex);
            }

            return LtiTokenValidationResult.Success(principal, jwt.Payload,
                jwt.Payload.GetClaimValueOrNull(Constants.LtiClaims.MessageType));
        }

        private LtiTokenValidationResult ValidateLtiClaims(JwtSecurityToken jwt)
        {
            // OpenID Connect Core §3.1.3.7: with more than one audience, azp is required; and
            // whenever azp is present, it must be this client. Without this a token minted for a
            // different tool in the same aud list would be accepted.
            var authorizedParty = jwt.Payload.Azp;
            if (jwt.Payload.Aud.Count > 1 && string.IsNullOrEmpty(authorizedParty))
            {
                return LtiTokenValidationResult.Failure(
                    "The id_token has multiple audiences but no azp claim.");
            }

            if (!string.IsNullOrEmpty(authorizedParty)
                && !string.Equals(authorizedParty, _options.ClientId, StringComparison.Ordinal))
            {
                return LtiTokenValidationResult.Failure(
                    "The id_token azp claim is not this tool's client id.");
            }

            // iat is required (OIDC Core §2, which LTI 1.3 builds on) and must not be in the future.
            // TokenValidationParameters only checks nbf and exp, so this is done here.
            if (!jwt.Payload.ContainsKey(JwtRegisteredClaimNames.Iat))
            {
                return LtiTokenValidationResult.Failure("The id_token has no iat claim.");
            }

            if (jwt.Payload.IssuedAt > DateTime.UtcNow + _options.ClockSkew)
            {
                return LtiTokenValidationResult.Failure("The id_token iat claim is in the future.");
            }

            if (_options.RequireLtiVersion)
            {
                var version = jwt.Payload.GetClaimValueOrNull(Constants.LtiClaims.Version);
                if (!string.Equals(version, Constants.Lti.Version, StringComparison.Ordinal))
                {
                    return LtiTokenValidationResult.Failure(
                        $"The id_token LTI version is '{version ?? "(missing)"}', expected '{Constants.Lti.Version}'.");
                }
            }

            if (string.IsNullOrEmpty(jwt.Payload.GetClaimValueOrNull(Constants.LtiClaims.MessageType)))
            {
                return LtiTokenValidationResult.Failure("The id_token has no message_type claim.");
            }

            var deploymentId = jwt.Payload.GetClaimValueOrNull(Constants.LtiClaims.DeploymentId);
            if (string.IsNullOrEmpty(deploymentId))
            {
                return LtiTokenValidationResult.Failure("The id_token has no deployment_id claim.");
            }

            if (_options.DeploymentIds != null && !_options.DeploymentIds.Contains(deploymentId))
            {
                return LtiTokenValidationResult.Failure(
                    $"The id_token deployment_id '{deploymentId}' is not registered for this tool.");
            }

            return null;
        }

        // Hash first so the comparison takes the same time whatever the lengths.
        private static bool FixedTimeEquals(string a, string b) =>
            System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(a)),
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(b)));

        private async Task<LtiTokenValidationResult> ValidateNonceAsync(JwtSecurityToken jwt, string expectedNonce,
            CancellationToken cancellationToken)
        {
            var nonce = jwt.Payload.Nonce;

            if (string.IsNullOrEmpty(nonce))
            {
                // A caller that expects a nonce gets a failure even when RequireNonce is off.
                return _options.RequireNonce || expectedNonce != null
                    ? LtiTokenValidationResult.Failure("The id_token has no nonce claim.")
                    : null;
            }

            if (expectedNonce != null && !FixedTimeEquals(nonce, expectedNonce))
            {
                return LtiTokenValidationResult.Failure("The id_token nonce does not match the one sent with the login.");
            }

            if (_options.NonceStore == null)
            {
                return null;
            }

            var firstUse = await _options.NonceStore
                .TryConsumeAsync(nonce, jwt.ValidTo, cancellationToken)
                .ConfigureAwait(false);

            return firstUse
                ? null
                : LtiTokenValidationResult.Failure("The id_token nonce has already been used.");
        }
    }

    internal static class JwtPayloadExtensions
    {
        /// <summary>
        /// Reads a claim as a string without deserializing, so a claim of an unexpected shape
        /// fails the check rather than throwing.
        /// </summary>
        public static string GetClaimValueOrNull(this JwtPayload payload, string type) =>
            payload.TryGetValue(type, out var value) ? value as string : null;
    }
}
