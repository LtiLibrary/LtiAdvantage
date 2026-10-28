using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.Jwks;
using LtiAdvantage.Jwks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class JwksController : JwksControllerBase
    {
        public JwksController(IJwksKeyStore keyStore, ILogger<JwksControllerBase> logger)
            : base(keyStore, logger) { }

        /// <summary>
        /// When the route is moved to carry a {registrationId}, publish that registration's key
        /// instead of the store's: the per-registration pattern GetPublicKeysAsync documents.
        /// </summary>
        protected override Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync()
        {
            if (!RouteData.Values.TryGetValue("registrationId", out var registrationId))
                return base.GetPublicKeysAsync();

            using var rsa = RSA.Create(2048);
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = "kid-" + registrationId });
            return Task.FromResult<IReadOnlyList<JsonWebKey>>(new[] { jwk });
        }
    }

    public class TestJwksKeyStore : IJwksKeyStore
    {
        // Deliberately returns a key WITH private parameters and a symmetric key, to prove the
        // controller publishes only public RSA/EC parameters whatever the store hands it.
        public Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync()
        {
            using var rsa = RSA.Create(2048);
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true)) { KeyId = "test-kid-1" });
            jwk.Use = "sig";
            jwk.Alg = "RS256";
            jwk.KeyOps.Add("sign");

            var symmetric = new JsonWebKey { Kty = "oct", Kid = "test-oct", K = Base64UrlEncoder.Encode(new byte[32]) };

            // An RSA key without its modulus is useless to verifiers and must be skipped.
            var incomplete = new JsonWebKey { Kty = "RSA", Kid = "test-incomplete", E = "AQAB" };

            // A public key without key_ops: published without an empty key_ops array.
            using var rsa2 = RSA.Create(2048);
            var publicOnly = JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(rsa2.ExportParameters(includePrivateParameters: false)) { KeyId = "test-kid-2" });

            // An EC key restricted to key agreement: no public counterpart, so it must be skipped
            // rather than published without restrictions.
            using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var ecParams = ec.ExportParameters(includePrivateParameters: false);
            var deriveOnly = new JsonWebKey
            {
                Kty = "EC", Kid = "test-derive", Crv = "P-256",
                X = Base64UrlEncoder.Encode(ecParams.Q.X), Y = Base64UrlEncoder.Encode(ecParams.Q.Y)
            };
            deriveOnly.KeyOps.Add("deriveKey");

            return Task.FromResult<IReadOnlyList<JsonWebKey>>(new[] { jwk, symmetric, incomplete, publicOnly, deriveOnly });
        }
    }
}
