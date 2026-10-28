using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.Jwks
{
    /// <summary>
    /// An <see cref="IJwksKeyStore"/> over a fixed set of keys held in memory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A reference implementation so that publishing a JWKS does not require writing a store
    /// first. It suits a deployment whose signing keys are loaded at startup and rotated by a
    /// restart. It is not suitable where keys rotate while the process runs, or where several
    /// instances must agree on the current key: implement <see cref="IJwksKeyStore"/> against
    /// whatever the instances share instead.
    /// </para>
    /// <para>
    /// Keys are sanitized on the way in, so a private JWK handed to this store by mistake is
    /// still published as public material only.
    /// </para>
    /// </remarks>
    public class InMemoryJwksKeyStore : IJwksKeyStore
    {
        private readonly IReadOnlyList<JsonWebKey> _publicKeys;

        /// <summary>
        /// Constructs a store over the supplied keys.
        /// </summary>
        /// <param name="keys">
        /// The keys to publish. Each must have a <see cref="JsonWebKey.Kid"/>, because a verifier
        /// selects a key by id. Private components are stripped.
        /// </param>
        public InMemoryJwksKeyStore(IEnumerable<JsonWebKey> keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            var sanitized = keys.Select(ToPublicKey).ToArray();

            if (sanitized.Any(k => string.IsNullOrEmpty(k.Kid)))
            {
                throw new ArgumentException("Every published key must have a Kid.", nameof(keys));
            }

            var duplicate = sanitized
                .GroupBy(k => k.Kid, StringComparer.Ordinal)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
            {
                throw new ArgumentException(
                    $"More than one published key has the Kid '{duplicate.Key}'.", nameof(keys));
            }

            _publicKeys = sanitized;
        }

        /// <summary>
        /// Constructs a store over the supplied keys.
        /// </summary>
        /// <param name="keys">The keys to publish.</param>
        public InMemoryJwksKeyStore(params JsonWebKey[] keys) : this((IEnumerable<JsonWebKey>) keys)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync() => Task.FromResult(_publicKeys);

        /// <summary>
        /// Builds a public signing JWK from an RSA key, ready to publish.
        /// </summary>
        /// <param name="rsa">
        /// The signing key. Only its public components are read, so passing a key that holds
        /// private material is safe.
        /// </param>
        /// <param name="keyId">
        /// The key's stable, unique id. It must match the "kid" header of the tokens signed with
        /// this key, otherwise verifiers cannot pair the two.
        /// </param>
        public static JsonWebKey CreateRsaKey(RSA rsa, string keyId)
        {
            if (rsa == null) throw new ArgumentNullException(nameof(rsa));
            if (string.IsNullOrWhiteSpace(keyId)) throw new ArgumentException("Kid is required", nameof(keyId));

            var parameters = rsa.ExportParameters(includePrivateParameters: false);

            return new JsonWebKey
            {
                Kty = JsonWebAlgorithmsKeyTypes.RSA,
                Use = "sig",
                Alg = SecurityAlgorithms.RsaSha256,
                Kid = keyId,
                N = Base64UrlEncoder.Encode(parameters.Modulus),
                E = Base64UrlEncoder.Encode(parameters.Exponent)
            };
        }

        /// <summary>
        /// Copies the public half of a key (see <see cref="PublicJsonWebKey.From"/>), and fills in
        /// <c>use</c> and <c>alg</c> when they are missing.
        /// </summary>
        private static JsonWebKey ToPublicKey(JsonWebKey key)
        {
            if (key == null) throw new ArgumentException("A published key must not be null.", nameof(key));

            var publicKey = PublicJsonWebKey.From(key)
                ?? throw new ArgumentException(
                    $"Key '{key.Kid}' cannot be published: it must be an RSA or EC key with all its public " +
                    "parameters, and its key_ops (if any) must allow a public use such as verify.", nameof(key));

            // use must agree with key_ops (RFC 7517 §4.3): only default it to "sig" when the key
            // is unrestricted or restricted to verification; "enc" for encryption-only keys.
            if (string.IsNullOrEmpty(publicKey.Use))
            {
                var ops = publicKey.KeyOps;
                if (ops.Count == 0 || ops.All(op => op == "verify")) publicKey.Use = "sig";
                else if (ops.All(op => op == "encrypt" || op == "wrapKey")) publicKey.Use = "enc";
                // Mixed operations: leave use unset rather than contradict key_ops.
            }
            // A signing algorithm only makes sense for a signing key; leave alg unset otherwise.
            if (string.IsNullOrEmpty(publicKey.Alg) && publicKey.Use == "sig")
            {
                publicKey.Alg = publicKey.Kty == JsonWebAlgorithmsKeyTypes.RSA
                    ? SecurityAlgorithms.RsaSha256
                    : DefaultEcAlgorithm(publicKey.Crv);
            }

            return publicKey;
        }

        /// <summary>The JWS algorithm that goes with an EC curve (RFC 7518 §3.4); null if unknown.</summary>
        private static string DefaultEcAlgorithm(string curve)
        {
            switch (curve)
            {
                case JsonWebKeyECTypes.P256: return SecurityAlgorithms.EcdsaSha256;
                case JsonWebKeyECTypes.P384: return SecurityAlgorithms.EcdsaSha384;
                case JsonWebKeyECTypes.P521: return SecurityAlgorithms.EcdsaSha512;
                default: return null;
            }
        }
    }
}
