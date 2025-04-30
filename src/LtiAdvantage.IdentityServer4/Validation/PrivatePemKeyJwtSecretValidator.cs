using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace LtiAdvantage.OpenIddict.Validation
{
    /// <summary>
    /// Authenticates a client using public key JWT client secrets.
    /// </summary>
    /// <remarks>
    /// See https://tools.ietf.org/html/rfc7523 and the IMS application profile
    /// https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant).
    /// </remarks>
    public class PrivatePemKeyJwtSecretValidator : IOpenIddictServerHandler<OpenIddictServerEvents.ValidateTokenRequestContext>
    {
        private readonly ILogger<PrivatePemKeyJwtSecretValidator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOpenIddictApplicationManager _applicationManager;

        /// <summary>
        /// Instantiates an instance of Signed JWT secret validator.
        /// </summary>
        public PrivatePemKeyJwtSecretValidator(
            IHttpContextAccessor httpContextAccessor, 
            ILogger<PrivatePemKeyJwtSecretValidator> logger,
            IOpenIddictApplicationManager applicationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _applicationManager = applicationManager;
        }

        public async ValueTask HandleAsync(OpenIddictServerEvents.ValidateTokenRequestContext context)
        {
            // Skip validation if the client_assertion_type is not supported
            if (string.IsNullOrEmpty(context.Request.ClientAssertionType) ||
                context.Request.ClientAssertionType != OpenIddictConstants.ClientAssertionTypes.JwtBearer)
            {
                return;
            }

            // Skip validation if the client_assertion is not present
            if (string.IsNullOrEmpty(context.Request.ClientAssertion))
            {
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "The client assertion cannot be null or empty.");

                return;
            }

            var token = context.Request.ClientAssertion;
            var handler = new JwtSecurityTokenHandler();
            
            if (!handler.CanReadToken(token))
            {
                _logger.LogError("Client assertion is not a well-formed JWT.");
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "Client assertion is not a well-formed JWT.");
                return;
            }

            // Extract claims from the token
            var jwtToken = handler.ReadJwtToken(token);
            var clientId = jwtToken.Subject;

            if (string.IsNullOrEmpty(clientId))
            {
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "The client identifier cannot be extracted from the client assertion.");
                return;
            }

            // Get the client application
            var application = await _applicationManager.FindByClientIdAsync(clientId);
            if (application == null)
            {
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "The client application cannot be found.");
                return;
            }

            // Collect PEM keys from client secrets
            var pemKeys = new List<string>();
            var properties = await _applicationManager.GetPropertiesAsync(application);
            foreach (var secret in properties)
            {
                if (secret.Key == Constants.SecretTypes.PublicPemKey)
                {
                    pemKeys.Add(secret.Value.ToString());
                }
            }

            if (!pemKeys.Any())
            {
                _logger.LogError("There are no keys available to validate the client assertion.");
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "There are no keys available to validate the client assertion.");
                return;
            }

            var rsaSecurityKeys = GetRsaSecurityKeys(pemKeys);
            var audienceUri = _httpContextAccessor.HttpContext.Request.Scheme + "://" + 
                _httpContextAccessor.HttpContext.Request.Host.Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                IssuerSigningKeys = rsaSecurityKeys,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidAudiences = new[]
                {
                    audienceUri,
                    string.Concat(audienceUri.TrimEnd('/'), "/connect/token")
                },
                ValidateAudience = true
            };

            try
            {
                handler.ValidateToken(token, tokenValidationParameters, out _);
                
                // Set the validated client ID in the context
                context.Request.ClientId = clientId;
                
                // Mark the request as validated to prevent other validators from running
                context.HandleRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                context.Reject(
                    error: OpenIddictConstants.Errors.InvalidClient,
                    description: "JWT token validation failed.");
            }
        }

        /// <summary>
        /// Get the PEM format secrets.
        /// </summary>
        /// <param name="pemKeys">The PEM key strings.</param>
        /// <returns>The PEM secrets converted into <see cref="RsaSecurityKey"/>'s.</returns>
        private static List<RsaSecurityKey> GetRsaSecurityKeys(IEnumerable<string> pemKeys)
        {
            var rsaSecurityKeys = new List<RsaSecurityKey>();

            foreach (var pemKey in pemKeys)
            {
                using (var keyTextReader = new StringReader(pemKey))
                {
                    // PemReader can read any PEM file. Only interested in RsaKeyParameters.
                    if (new PemReader(keyTextReader).ReadObject() is RsaKeyParameters bouncyKeyParameters)
                    {
                        var rsaParameters = new RSAParameters
                        {
                            Modulus = bouncyKeyParameters.Modulus.ToByteArrayUnsigned(),
                            Exponent = bouncyKeyParameters.Exponent.ToByteArrayUnsigned()
                        };

                        var rsaSecurityKey = new RsaSecurityKey(rsaParameters);
                        rsaSecurityKeys.Add(rsaSecurityKey);
                    }
                }
            }

            return rsaSecurityKeys;
        }
    }
}
