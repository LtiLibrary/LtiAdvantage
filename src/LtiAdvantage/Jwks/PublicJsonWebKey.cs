using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.Jwks
{
    /// <summary>
    /// Copies the public half of a JSON Web Key, for publishing in a JWKS. Shared by
    /// <c>JwksControllerBase</c> and <see cref="InMemoryJwksKeyStore"/>, so both apply the same rules.
    /// </summary>
    public static class PublicJsonWebKey
    {
        /// <summary>
        /// Returns a copy of <paramref name="key"/> with only its public parameters, or <c>null</c>
        /// when the key cannot be published: it is not an RSA or EC key (for example a symmetric
        /// key), it is missing public parameters (<c>n</c>/<c>e</c> or <c>crv</c>/<c>x</c>/<c>y</c>)
        /// that verifiers need, or <c>key_ops</c> restricts it to operations with no public
        /// counterpart.
        /// </summary>
        /// <remarks>
        /// Private parameters (<c>d</c>, <c>p</c>, <c>q</c>, <c>dp</c>, <c>dq</c>, <c>qi</c>,
        /// <c>k</c>, <c>oth</c>) are never copied. <c>key_ops</c> are mapped to what the public key
        /// may do (<c>sign</c> becomes <c>verify</c>, and so on), keeping the key's restriction.
        /// </remarks>
        public static JsonWebKey From(JsonWebKey key)
        {
            if (key == null) return null;

            var publicKey = new JsonWebKey
            {
                Kty = key.Kty,
                Kid = key.Kid,
                Use = key.Use,
                Alg = key.Alg,
                X5t = key.X5t,
                X5tS256 = key.X5tS256,
                X5u = key.X5u
            };

            // If the key is restricted to operations that have no public counterpart (e.g. deriveKey),
            // it is not published: without key_ops it would be usable for more than intended.
            var publicOps = key.KeyOps.Select(ToPublicOperation).Where(o => o != null).Distinct().ToList();
            if (key.KeyOps.Count > 0 && publicOps.Count == 0) return null;
            foreach (var op in publicOps) publicKey.KeyOps.Add(op);
            foreach (var cert in key.X5c) publicKey.X5c.Add(cert);

            switch (key.Kty)
            {
                case JsonWebAlgorithmsKeyTypes.RSA:
                    if (string.IsNullOrEmpty(key.N) || string.IsNullOrEmpty(key.E)) return null;
                    publicKey.N = key.N;
                    publicKey.E = key.E;
                    return publicKey;
                case JsonWebAlgorithmsKeyTypes.EllipticCurve:
                    if (string.IsNullOrEmpty(key.Crv) || string.IsNullOrEmpty(key.X) || string.IsNullOrEmpty(key.Y)) return null;
                    publicKey.Crv = key.Crv;
                    publicKey.X = key.X;
                    publicKey.Y = key.Y;
                    return publicKey;
                default:
                    return null;
            }
        }

        private static string ToPublicOperation(string operation)
        {
            switch (operation)
            {
                case "sign":
                case "verify":
                    return "verify";
                case "decrypt":
                case "encrypt":
                    return "encrypt";
                case "unwrapKey":
                case "wrapKey":
                    return "wrapKey";
                default:
                    return null;
            }
        }
    }
}
