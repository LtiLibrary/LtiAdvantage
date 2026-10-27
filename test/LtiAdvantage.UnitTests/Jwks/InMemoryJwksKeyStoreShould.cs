using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.Jwks;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LtiAdvantage.UnitTests.Jwks
{
    public class InMemoryJwksKeyStoreShould
    {
        [Fact]
        public async Task PublishTheSuppliedKeys()
        {
            using var rsa = RSA.Create(2048);
            var store = new InMemoryJwksKeyStore(InMemoryJwksKeyStore.CreateRsaKey(rsa, "key-1"));

            var keys = await store.GetPublicKeysAsync();

            var key = Assert.Single(keys);
            Assert.Equal("key-1", key.Kid);
            Assert.Equal("RSA", key.Kty);
            Assert.Equal("sig", key.Use);
            Assert.Equal("RS256", key.Alg);
            Assert.False(string.IsNullOrEmpty(key.N));
            Assert.False(string.IsNullOrEmpty(key.E));
        }

        /// <summary>
        /// The key built from an RSA key must carry the public modulus and exponent only, even
        /// though the RSA instance it was built from holds the private key too.
        /// </summary>
        [Fact]
        public async Task NotPublishPrivateMaterial_FromAnRsaKey()
        {
            using var rsa = RSA.Create(2048);
            var store = new InMemoryJwksKeyStore(InMemoryJwksKeyStore.CreateRsaKey(rsa, "key-1"));

            var json = JsonSerializer.Serialize(await store.GetPublicKeysAsync());

            AssertNoPrivateFields(json);
        }

        /// <summary>
        /// Handing the store a private JWK is an easy mistake to make and a catastrophic one to
        /// publish, so the store strips the private components rather than trusting the caller.
        /// </summary>
        [Fact]
        public async Task StripPrivateMaterial_FromAPrivateJwk()
        {
            using var rsa = RSA.Create(2048);
            var privateParameters = rsa.ExportParameters(includePrivateParameters: true);

            var privateJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(privateParameters));
            privateJwk.Kid = "key-1";
            Assert.False(string.IsNullOrEmpty(privateJwk.D), "the test fixture should hold a private key");

            var store = new InMemoryJwksKeyStore(privateJwk);
            var keys = await store.GetPublicKeysAsync();

            var key = Assert.Single(keys);
            Assert.Null(key.D);
            Assert.Null(key.P);
            Assert.Null(key.Q);
            Assert.Null(key.DP);
            Assert.Null(key.DQ);
            Assert.Null(key.QI);
            Assert.Null(key.K);

            // The public half survives, otherwise nothing could verify a signature.
            Assert.Equal(privateJwk.N, key.N);
            Assert.Equal(privateJwk.E, key.E);

            AssertNoPrivateFields(JsonSerializer.Serialize(keys));
        }

        /// <summary>
        /// A verifier picks a key by id, so a key without one is unusable and two keys sharing
        /// one are ambiguous. Both are worth catching at startup rather than at launch.
        /// </summary>
        [Fact]
        public void RejectKeysThatCannotBeSelected()
        {
            using var rsa = RSA.Create(2048);

            var noKid = InMemoryJwksKeyStore.CreateRsaKey(rsa, "key-1");
            noKid.Kid = null;
            Assert.Throws<ArgumentException>(() => new InMemoryJwksKeyStore(noKid));

            Assert.Throws<ArgumentException>(() => new InMemoryJwksKeyStore(
                InMemoryJwksKeyStore.CreateRsaKey(rsa, "same"),
                InMemoryJwksKeyStore.CreateRsaKey(rsa, "same")));

            Assert.Throws<ArgumentNullException>(() => new InMemoryJwksKeyStore(null));
            Assert.Throws<ArgumentNullException>(() => InMemoryJwksKeyStore.CreateRsaKey(null, "key-1"));
            Assert.Throws<ArgumentException>(() => InMemoryJwksKeyStore.CreateRsaKey(rsa, " "));
        }

        /// <summary>
        /// A symmetric key has no public half; published, it would carry no key material at all.
        /// </summary>
        [Fact]
        public void RejectKeysWithoutAPublicHalf()
        {
            var symmetric = new JsonWebKey { Kty = "oct", Kid = "hmac", K = Base64UrlEncoder.Encode(new byte[32]) };
            Assert.Throws<ArgumentException>(() => new InMemoryJwksKeyStore(symmetric));

            var noModulus = new JsonWebKey { Kty = "RSA", Kid = "rsa", E = "AQAB" };
            Assert.Throws<ArgumentException>(() => new InMemoryJwksKeyStore(noModulus));
        }

        /// <summary>key_ops keep their restriction, mapped to the public operation.</summary>
        [Fact]
        public async Task MapKeyOps_ToThePublicOperation()
        {
            using var rsa = RSA.Create(2048);
            var key = InMemoryJwksKeyStore.CreateRsaKey(rsa, "key-1");
            key.KeyOps.Add("sign");

            var published = (await new InMemoryJwksKeyStore(key).GetPublicKeysAsync()).Single();

            Assert.Equal(new[] {"verify"}, published.KeyOps);
        }

        /// <summary>A missing use is filled in to agree with key_ops (RFC 7517 §4.3).</summary>
        [Theory]
        [InlineData(null, "sig")]
        [InlineData("sign", "sig")]
        [InlineData("decrypt", "enc")]
        public async Task DefaultUse_ConsistentWithKeyOps(string keyOp, string expectedUse)
        {
            using var rsa = RSA.Create(2048);
            var key = InMemoryJwksKeyStore.CreateRsaKey(rsa, "key-1");
            key.Use = null;
            if (keyOp != null) key.KeyOps.Add(keyOp);

            var published = (await new InMemoryJwksKeyStore(key).GetPublicKeysAsync()).Single();

            Assert.Equal(expectedUse, published.Use);
        }

        /// <summary>An EC key without alg gets the algorithm for its curve, not RS256.</summary>
        [Theory]
        [InlineData("P-256", "ES256")]
        [InlineData("P-384", "ES384")]
        public async Task DefaultTheAlgorithm_FromTheCurve(string curveName, string expectedAlg)
        {
            var curve = curveName == "P-256" ? ECCurve.NamedCurves.nistP256 : ECCurve.NamedCurves.nistP384;
            using var ec = ECDsa.Create(curve);
            var q = ec.ExportParameters(includePrivateParameters: false).Q;
            var key = new JsonWebKey
            {
                Kty = "EC", Kid = "ec-1", Crv = curveName,
                X = Base64UrlEncoder.Encode(q.X), Y = Base64UrlEncoder.Encode(q.Y)
            };

            var published = (await new InMemoryJwksKeyStore(key).GetPublicKeysAsync()).Single();

            Assert.Equal(expectedAlg, published.Alg);
        }

        /// <summary>
        /// Rotation means serving the retiring key alongside the current one until every token
        /// signed with it has expired.
        /// </summary>
        [Fact]
        public async Task PublishSeveralKeysDuringRotation()
        {
            using var current = RSA.Create(2048);
            using var retiring = RSA.Create(2048);

            var store = new InMemoryJwksKeyStore(
                InMemoryJwksKeyStore.CreateRsaKey(current, "key-2"),
                InMemoryJwksKeyStore.CreateRsaKey(retiring, "key-1"));

            var keys = await store.GetPublicKeysAsync();

            Assert.Equal(new[] {"key-2", "key-1"}, keys.Select(k => k.Kid));
        }

        /// <summary>
        /// Asserts that no private component carries a value. Microsoft's JsonWebKey initialises
        /// its collection properties, so "oth" is serialized as an empty array whether or not a
        /// private key was involved; an empty array holds no key material, so the assertion is
        /// about values rather than the mere presence of the field.
        /// </summary>
        private static void AssertNoPrivateFields(string json)
        {
            using var document = JsonDocument.Parse(json);

            foreach (var key in document.RootElement.EnumerateArray())
            {
                foreach (var field in new[] {"d", "p", "q", "dp", "dq", "qi", "k", "oth"})
                {
                    if (!key.TryGetProperty(field, out var value))
                    {
                        continue;
                    }

                    var isEmpty = value.ValueKind == JsonValueKind.Null
                                  || (value.ValueKind == JsonValueKind.Array && !value.EnumerateArray().Any())
                                  || (value.ValueKind == JsonValueKind.String
                                      && string.IsNullOrEmpty(value.GetString()));

                    Assert.True(isEmpty,
                        $"the published JWKS must not contain a value for the '{field}' field");
                }
            }
        }
    }
}
