using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// The outcome of validating an LTI ID token.
    /// </summary>
    public class LtiTokenValidationResult
    {
        private LtiTokenValidationResult()
        {
        }

        /// <summary>
        /// Whether the token passed every check. When false, nothing but <see cref="Error"/>
        /// is populated and no claim on the token may be trusted.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Why validation failed, or null when it succeeded.
        /// </summary>
        /// <remarks>
        /// Suitable for a log. Do not return it to the browser: it distinguishes failure modes
        /// that an attacker probing the endpoint would otherwise have to guess at.
        /// </remarks>
        public string Error { get; private set; }

        /// <summary>
        /// The validated claims, or null when validation failed.
        /// </summary>
        public ClaimsPrincipal Principal { get; private set; }

        /// <summary>
        /// The validated token payload, or null when validation failed. Pass it to the
        /// constructor of the message type named by <see cref="MessageType"/>, for example
        /// <c>new LtiResourceLinkRequest(result.Payload)</c>.
        /// </summary>
        public JwtPayload Payload { get; private set; }

        /// <summary>
        /// The value of the LTI message_type claim, or null when validation failed.
        /// </summary>
        public string MessageType { get; private set; }

        /// <summary>
        /// The exception behind a failure, when there was one (for example the JWKS fetch error),
        /// for logging. Null on success and for failures that are plain claim checks.
        /// </summary>
        public Exception Exception { get; private set; }

        internal static LtiTokenValidationResult Success(ClaimsPrincipal principal, JwtPayload payload,
            string messageType) =>
            new()
            {
                IsValid = true,
                Principal = principal,
                Payload = payload,
                MessageType = messageType
            };

        internal static LtiTokenValidationResult Failure(string error, Exception exception) =>
            new() {IsValid = false, Error = error, Exception = exception};

        internal static LtiTokenValidationResult Failure(string error) =>
            new() {IsValid = false, Error = error};
    }
}
