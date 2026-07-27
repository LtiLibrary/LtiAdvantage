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
        public Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync()
        {
            using var rsa = RSA.Create(2048);
            var rsaParams = rsa.ExportParameters(includePrivateParameters: false);
            var jwk = new JsonWebKey
            {
                Kty = "RSA",
                Use = "sig",
                Alg = "RS256",
                Kid = "test-kid-1",
                N = Base64UrlEncoder.Encode(rsaParams.Modulus),
                E = Base64UrlEncoder.Encode(rsaParams.Exponent),
            };
            return Task.FromResult<IReadOnlyList<JsonWebKey>>(new[] { jwk });
        }
    }
}
