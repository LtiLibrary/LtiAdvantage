using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LtiAdvantage.UnitTests.IdentityModel
{
    public class RsaPemKeysShould
    {
        [Fact]
        public void GenerateAPairThatSignsAndVerifies()
        {
            var pair = RsaPemKeys.GenerateKeyPair();

            Assert.StartsWith("-----BEGIN RSA PRIVATE KEY-----", pair.PrivateKey);
            Assert.StartsWith("-----BEGIN PUBLIC KEY-----", pair.PublicKey);
            Assert.False(string.IsNullOrEmpty(pair.KeyId));

            var credentials = RsaPemKeys.SigningCredentialsFromPem(pair.PrivateKey, pair.KeyId);
            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                new JwtHeader(credentials), new JwtPayload {["sub"] = "x"}));

            Assert.Equal(pair.KeyId, new JwtSecurityTokenHandler().ReadJwtToken(jwt).Header.Kid);
            new JwtSecurityTokenHandler().ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateAudience = false, ValidateIssuer = false, ValidateLifetime = false,
                IssuerSigningKey = RsaPemKeys.PublicKeyFromPem(pair.PublicKey)
            }, out _);
        }

        [Fact]
        public void ReadPkcs8PrivateKeys()
        {
            using var rsa = RSA.Create(2048);

            var credentials = RsaPemKeys.SigningCredentialsFromPem(rsa.ExportPkcs8PrivateKeyPem(), "k1");

            Assert.Equal("k1", credentials.Key.KeyId);
            Assert.Equal(SecurityAlgorithms.RsaSha256, credentials.Algorithm);
        }

        [Fact]
        public void RefuseAPublicKey_AsSigningKey()
        {
            var pair = RsaPemKeys.GenerateKeyPair();

            Assert.ThrowsAny<CryptographicException>(() => RsaPemKeys.SigningCredentialsFromPem(pair.PublicKey, "k1"));
        }

        /// <summary>A stored key may be the private key followed by its public key; both reads still work.</summary>
        [Fact]
        public void ReadAPemThatHoldsBothKeys()
        {
            var pair = RsaPemKeys.GenerateKeyPair();
            var both = pair.PrivateKey + "\n" + pair.PublicKey;

            Assert.NotNull(RsaPemKeys.SigningCredentialsFromPem(both, "k1"));
            Assert.NotNull(RsaPemKeys.PublicKeyFromPem(both));
            Assert.NotNull(RsaPemKeys.PublicKeyFromPem(pair.PrivateKey));
        }

        [Fact]
        public async Task ReportAnUnreadableKey_OnTheTokenResponse()
        {
            var handler = new CapturingHandler();

            var response = await AccessTokenUtil.GetAccessTokenAsync(new HttpClient(handler), "https://platform.example.com",
                new[] {"s"}, "client-1", "https://platform.example.com/token", "not a key", "k1");

            Assert.True(response.IsError);
            Assert.Null(handler.Request);
        }

#pragma warning disable LTIADV001, LTIADV002 // Obsolete members must keep working.
        [Fact]
        public void KeepTheObsoletePemHelperWorking()
        {
            var pair = PemHelper.GenerateRsaKeyPair();

            Assert.False(string.IsNullOrEmpty(pair.KeyId));
            Assert.Null(PemHelper.SigningCredentialsFromPemString(pair.PrivateKey).Key.KeyId);
            Assert.Equal(pair.KeyId, PemHelper.SigningCredentialsFromPemString(pair.PrivateKey, pair.KeyId).Key.KeyId);
            Assert.NotNull(PemHelper.PublicKeyFromPemString(pair.PublicKey));
        }
#pragma warning restore LTIADV001, LTIADV002

        /// <summary>The client assertion's iat is a NumericDate number and the header carries the kid.</summary>
        [Fact]
        public async Task SendAClientAssertion_WithNumericIatAndKid()
        {
            var pair = RsaPemKeys.GenerateKeyPair();
            var handler = new CapturingHandler();

            await AccessTokenUtil.GetAccessTokenAsync(new HttpClient(handler), "https://platform.example.com",
                new[] {"scope-a", "scope-b"}, "client-1", "https://platform.example.com/token", pair.PrivateKey, pair.KeyId);

            var form = await handler.Request.Content.ReadAsStringAsync();
            var assertion = Uri.UnescapeDataString(System.Text.RegularExpressions.Regex
                .Match(form, "client_assertion=([^&]+)").Groups[1].Value);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(assertion);

            Assert.Equal(pair.KeyId, jwt.Header.Kid);
            using var payload = JsonDocument.Parse(Base64UrlEncoder.Decode(assertion.Split('.')[1]));
            foreach (var claim in new[] {"iat", "nbf", "exp"})
                Assert.Equal(JsonValueKind.Number, payload.RootElement.GetProperty(claim).ValueKind);
            Assert.Equal("client-1", payload.RootElement.GetProperty("iss").GetString());
            Assert.Equal("https://platform.example.com/token", handler.Request.RequestUri.AbsoluteUri);
        }

        private sealed class CapturingHandler : HttpMessageHandler
        {
            public HttpRequestMessage Request { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Request = request;
                await request.Content.LoadIntoBufferAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"access_token\":\"t\",\"token_type\":\"Bearer\",\"expires_in\":3600}")
                };
            }
        }
    }
}
