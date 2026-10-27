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
        /// Copies the public half of a key, leaving every private component behind.
        /// </summary>
        private static JsonWebKey ToPublicKey(JsonWebKey key)
        {
            if (key == null) throw new ArgumentException("A published key must not be null.", nameof(key));

            // Only asymmetric signing keys can be published: a symmetric (oct) key has no public
            // half, so it would go out with no key material at all.
            string defaultAlg;
            switch (key.Kty)
            {
                case JsonWebAlgorithmsKeyTypes.RSA:
                    if (string.IsNullOrEmpty(key.N) || string.IsNullOrEmpty(key.E))
                        throw new ArgumentException($"RSA key '{key.Kid}' has no public modulus or exponent.", nameof(key));
                    defaultAlg = SecurityAlgorithms.RsaSha256;
                    break;
                case JsonWebAlgorithmsKeyTypes.EllipticCurve:
                    if (string.IsNullOrEmpty(key.Crv) || string.IsNullOrEmpty(key.X) || string.IsNullOrEmpty(key.Y))
                        throw new ArgumentException($"EC key '{key.Kid}' has no curve or public point.", nameof(key));
                    defaultAlg = DefaultEcAlgorithm(key.Crv);
                    break;
                default:
                    throw new ArgumentException(
                        $"Key '{key.Kid}' has type '{key.Kty}'; only RSA and EC keys can be published.", nameof(key));
            }

            var publicKey = new JsonWebKey
            {
                Kty = key.Kty,
                Use = string.IsNullOrEmpty(key.Use) ? "sig" : key.Use,
                Alg = string.IsNullOrEmpty(key.Alg) ? defaultAlg : key.Alg,
                Kid = key.Kid,

                // RSA public components.
                N = key.N,
                E = key.E,

                // EC public components, for a deployment that signs with ES256 rather than RS256.
                Crv = key.Crv,
                X = key.X,
                Y = key.Y
            };

            foreach (var certificate in key.X5c)
            {
                publicKey.X5c.Add(certificate);
            }

            publicKey.X5t = key.X5t;
            publicKey.X5u = key.X5u;

            // D, P, Q, DP, DQ, QI, K and Oth are deliberately not copied. Publishing any of them
            // would hand out the signing key.
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
