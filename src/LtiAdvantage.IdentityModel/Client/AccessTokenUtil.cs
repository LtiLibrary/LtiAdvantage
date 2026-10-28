using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
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
        private static readonly HttpClient SharedHttpClient = new HttpClient();

        /// <summary>
        /// Get an access token from the issuer, using a shared <see cref="HttpClient"/> and no key id.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="scopes">The scopes to request.</param>
        /// <param name="clientId">The tool's client identifier.</param>
        /// <param name="accessTokenUrl">The platform's access token url.</param>
        /// <param name="privateKey">The tool's private key.</param>
        /// <returns>The token response.</returns>
        [Obsolete("Pass an HttpClient (for example from IHttpClientFactory) and the key id: " +
                  "GetAccessTokenAsync(client, issuer, scopes, clientId, accessTokenUrl, privateKey, keyId).",
            DiagnosticId = "LTIADV002")]
        public static Task<TokenResponse> GetAccessTokenAsync(string issuer, string[] scopes, string clientId,
            string accessTokenUrl, string privateKey) =>
            GetAccessTokenAsync(SharedHttpClient, issuer, scopes, clientId, accessTokenUrl, privateKey, null);

        /// <summary>
        /// Get an access token from the issuer with a signed JWT client assertion.
        /// </summary>
        /// <param name="client">The HTTP client used for the token request.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="scopes">The scopes to request.</param>
        /// <param name="clientId">The tool's client identifier.</param>
        /// <param name="accessTokenUrl">The platform's access token url.</param>
        /// <param name="privateKey">The tool's private key (PEM).</param>
        /// <param name="keyId">
        /// The id the key is published under in the tool's JWKS, written as the assertion's
        /// <c>kid</c> so the platform can pick the right key after a rotation.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The token response.</returns>
        public static async Task<TokenResponse> GetAccessTokenAsync(HttpClient client, string issuer, string[] scopes,
            string clientId, string accessTokenUrl, string privateKey, string keyId,
            CancellationToken cancellationToken = default)
        {
            if (client == null)
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(client)));
            }

            if (issuer.IsMissing())
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(issuer)));
            }

            if (scopes == null)
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(scopes)));
            }

            if (clientId.IsMissing())
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(clientId)));
            }

            if (accessTokenUrl.IsMissing())
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(accessTokenUrl)));
            }

            if (privateKey.IsMissing())
            {
                return ProtocolResponse.FromException<TokenResponse>(new ArgumentNullException(nameof(privateKey)));
            }

            string jwt;
            try
            {
                jwt = CreateClientAssertion(clientId, accessTokenUrl, privateKey, keyId);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is CryptographicException)
            {
                // An unreadable key is a bad input like the others: report it on the response.
                return ProtocolResponse.FromException<TokenResponse>(ex);
            }

            return await client.RequestClientCredentialsTokenWithJwtAsync(
                    new JwtClientCredentialsTokenRequest
                    {
                        Address = accessTokenUrl,
                        ClientId = clientId,
                        Jwt = jwt,
                        Scope = string.Join(" ", scopes)
                    }, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Builds the signed JWT the tool sends as its client credentials.
        /// </summary>
        private static string CreateClientAssertion(string clientId, string accessTokenUrl, string privateKey,
            string keyId)
        {
            var now = DateTime.UtcNow;
            // iat, nbf and exp are NumericDate numbers (RFC 7519), not strings.
            var payload = new JwtPayload();
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Iss, clientId));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, clientId));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Aud, accessTokenUrl));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Iat,
                EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Nbf,
                EpochTime.GetIntDate(now.AddSeconds(-5)).ToString(), ClaimValueTypes.Integer64));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Exp,
                EpochTime.GetIntDate(now.AddMinutes(5)).ToString(), ClaimValueTypes.Integer64));
            payload.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Convert.ToBase64String(CryptoRandom.CreateRandomKey(32))));

            var credentials = RsaPemKeys.SigningCredentialsFromPem(privateKey, keyId);
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(new JwtHeader(credentials), payload));
        }
    }
}
