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

            var symmetric = new JsonWebKey { Kty = "oct", Kid = "test-oct", K = Base64UrlEncoder.Encode(new byte[32]) };

            return Task.FromResult<IReadOnlyList<JsonWebKey>>(new[] { jwk, symmetric });
        }
    }
}
