using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using LtiAdvantage.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// Static utility to get an access token from the issuer.
    /// </summary>
    public static class AccessTokenUtil
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// Get an access token from the issuer.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="scopes">The scopes to request.</param>
        /// <param name="clientId">The tool's client identifier.</param>
        /// <param name="accessTokenUrl">The platform's access token url.</param>
        /// <param name="privateKey">The tool's private key.</param>
        /// <returns>The token response.</returns>
        public static async Task<TokenResponse> GetAccessTokenAsync(string issuer, string[] scopes, string clientId, string accessTokenUrl, string privateKey)
        {
            if (issuer.IsMissing())
            {
                return new TokenResponse(new ArgumentNullException(nameof(issuer)));
            }

            if (scopes == null)
            {
                return new TokenResponse(new ArgumentNullException(nameof(scopes)));
            }

            if (clientId.IsMissing())
            {
                return new TokenResponse(new ArgumentNullException(nameof(clientId)));
            }

            if (accessTokenUrl.IsMissing())
            {
                return new TokenResponse(new ArgumentNullException(nameof(accessTokenUrl)));
            }

            if (privateKey.IsMissing())
            {
                return new TokenResponse(new ArgumentNullException(nameof(privateKey)));
            }

            // Use a signed JWT as client credentials.
            var payload = new JwtPayload();
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Iss, clientId));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, clientId));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Aud, accessTokenUrl));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString()));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Nbf,
                EpochTime.GetIntDate(DateTime.UtcNow.AddSeconds(-5)).ToString(), ClaimValueTypes.Integer64));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Exp,
                EpochTime.GetIntDate(DateTime.UtcNow.AddMinutes(5)).ToString(), ClaimValueTypes.Integer64));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, CryptoRandom.CreateRandomKeyString(32)));

            var handler = new JwtSecurityTokenHandler();
            var credentials = PemHelper.SigningCredentialsFromPemString(privateKey);
            var jwt = handler.WriteToken(new JwtSecurityToken(new JwtHeader(credentials), payload));

            return await HttpClient.RequestClientCredentialsTokenWithJwtAsync(
                    new JwtClientCredentialsTokenRequest
                    {
                        Address = accessTokenUrl,
                        ClientId = clientId,
                        Jwt = jwt,
                        Scope = string.Join(" ", scopes)
                    });
        }
    }
}
