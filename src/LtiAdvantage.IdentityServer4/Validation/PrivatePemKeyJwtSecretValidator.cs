using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace LtiAdvantage.IdentityServer4.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// Authenticates a client using public key JWT client secrets.
    /// </summary>
    /// <remarks>
    /// See https://tools.ietf.org/html/rfc7523 and the IMS application profile
    /// https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant).
    /// This similar to <see cref="PrivateKeyJwtSecretValidator"/> with these differences:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Does not require that iss=sub. The IMS application profile does not require this.
    /// </description>        
    /// </item>
    /// <item>
    /// <description>
    /// Accepts either the token endpoint or that base url of the authentication server per
    /// the IMS application profile.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Uses serialized JSON Web Keys or PEM format keys instead of the full (leaf)
    /// certificate as base64.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class PrivatePemKeyJwtSecretValidator : ISecretValidator
    {
        private readonly ILogger<PrivatePemKeyJwtSecretValidator> _logger;
        private readonly string _audienceUri;

        /// <summary>
        /// Instantiates an instance of Signed JWT secret validator.
        /// </summary>
        public PrivatePemKeyJwtSecretValidator(IHttpContextAccessor contextAccessor, 
            ILogger<PrivatePemKeyJwtSecretValidator> logger)
        {
            _audienceUri = contextAccessor.HttpContext.GetIdentityServerIssuerUri();
            _logger = logger;
        }

        /// <inheritdoc />
        /// <summary>
        /// Validates the signed JWT.
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

            if (!(parsedSecret.Credential is string token))
            {
                _logger.LogError("ParsedSecret.Credential is not a string.");
                return fail;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                _logger.LogError("ParsedSecret.Credential is not a well formed JWT.");
                return fail;
            }

            // Collect the potential public keys from the client secrets
            var secretArray = secrets as Secret[] ?? secrets.ToArray();
            var pemKeys = GetPemKeys(secretArray);

            if (!pemKeys.Any())
            {
                _logger.LogError("There are no keys available to validate the client assertion.");
                return fail;
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The token must be signed to prove the client credentials.
                RequireSignedTokens = true,
                RequireExpirationTime = true,

                IssuerSigningKeys = pemKeys,
                ValidateIssuerSigningKey = true,

                // IMS recommendation is to send any unique name as Issuer. The IMS reference 
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
                handler.ValidateToken(token, tokenValidationParameters, out _);

                return success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "JWT token validation error");
                return fail;
            }
        }

        /// <summary>
        /// Get the PEM format secrets.
        /// </summary>
        /// <param name="secrets">The secrets to examine.</param>
        /// <returns>The PEM secrets converted into <see cref="RsaSecurityKey"/>'s.</returns>
        private static List<RsaSecurityKey> GetPemKeys(IEnumerable<Secret> secrets)
        {
            var pemKeys = secrets
                .Where(s => s.Type == Constants.SecretTypes.PublicPemKey)
                .Select(s => s.Value)
                .ToList();

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
