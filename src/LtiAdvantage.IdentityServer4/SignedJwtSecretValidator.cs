using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LtiAdvantageLibrary.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityServer4
{
    /// <inheritdoc />
    /// <summary>
    /// Validates a secret based on RS256 signed JWT token
    /// </summary>
    public class SignedJwtSecretValidator : ISecretValidator
    {
        private readonly ILogger<SignedJwtSecretValidator> _logger;
        private readonly string _audienceUri;

        /// <summary>
        /// Instantiates an instance of Signed JWT secret validator
        /// </summary>
        public SignedJwtSecretValidator(IHttpContextAccessor contextAccessor, ILogger<SignedJwtSecretValidator> logger)
        {
            _audienceUri = contextAccessor.HttpContext.GetIdentityServerIssuerUri();
            _logger = logger;
        }

        /// <inheritdoc />
        /// <summary>
        /// Validates a secret
        /// </summary>
        /// <param name="secrets">The stored secrets.</param>
        /// <param name="parsedSecret">The received secret.</param>
        /// <returns>
        /// A validation result
        /// </returns>
        /// <exception cref="T:System.ArgumentException">ParsedSecret.Credential is not a JWT token</exception>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.JwtBearer)
            {
                return fail;
            }

            if (!(parsedSecret.Credential is string jwt))
            {
                _logger.LogError("ParsedSecret.Credential is not a string.");
                return fail;
            }

            var publicKeys = secrets
                .Where(s => s.Type == Constants.SecretTypes.PublicKey)
                .Select(s => RsaHelper.PublicKeyFromPemString(s.Value))
                .ToList();

            if (!publicKeys.Any())
            {
                _logger.LogError("There are no public keys available to validate the client assertion.");
                return fail;
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The token must be signed to prove the client credentials.
                RequireSignedTokens = true,
                RequireExpirationTime = true,

                IssuerSigningKeys = publicKeys,
                ValidateIssuerSigningKey = true,

                // IMS recommendation is to send any unique name as Issuer. IMS reference 
                // implementation sends the tool name. The tool's own name for this client
                // is not known by the platform and cannot be validated.
                ValidateIssuer = false,

                // IMS recommendation is to send the base url of the authentication server
                // or the token URL.
                ValidAudiences = new []
                {
                    _audienceUri, 
                    string.Concat(_audienceUri.EnsureTrailingSlash(), "connect/token")
                },
                ValidateAudience = true
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(jwt, tokenValidationParameters, out _);

                return success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return fail;
            }
        }
    }
}
