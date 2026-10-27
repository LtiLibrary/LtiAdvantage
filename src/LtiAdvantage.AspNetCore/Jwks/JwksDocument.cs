using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>
    /// The JWKS document published by <see cref="JwksControllerBase"/> (RFC 7517 §5).
    /// </summary>
    /// <remarks>
    /// Used instead of <see cref="JsonWebKeySet"/> because IdentityModel's serializer writes empty
    /// <c>key_ops</c>, <c>x5c</c> and <c>oth</c> arrays for every key, which strict verifiers can reject.
    /// This type writes only the public members that are set.
    /// </remarks>
    public class JwksDocument
    {
        /// <summary>The published public keys.</summary>
        [JsonPropertyName("keys")]
        public IList<PublishedJsonWebKey> Keys { get; set; } = new List<PublishedJsonWebKey>();
    }

    /// <summary>A public JSON Web Key as published in a <see cref="JwksDocument"/>.</summary>
    public class PublishedJsonWebKey
    {
        /// <summary>Copies the public members of <paramref name="key"/>; empty lists are left out.</summary>
        public static PublishedJsonWebKey From(JsonWebKey key) => new PublishedJsonWebKey
        {
            Kty = key.Kty,
            Kid = key.Kid,
            Use = key.Use,
            Alg = key.Alg,
            KeyOps = key.KeyOps.Count > 0 ? key.KeyOps.ToList() : null,
            N = key.N,
            E = key.E,
            Crv = key.Crv,
            X = key.X,
            Y = key.Y,
            X5c = key.X5c.Count > 0 ? key.X5c.ToList() : null,
            X5t = key.X5t,
            X5tS256 = key.X5tS256,
            X5u = key.X5u
        };

        /// <summary>Key type (<c>RSA</c> or <c>EC</c>).</summary>
        [JsonPropertyName("kty")] public string Kty { get; set; }

        /// <summary>Key ID.</summary>
        [JsonPropertyName("kid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Kid { get; set; }

        /// <summary>Public key use, e.g. <c>sig</c>.</summary>
        [JsonPropertyName("use"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Use { get; set; }

        /// <summary>Algorithm, e.g. <c>RS256</c>.</summary>
        [JsonPropertyName("alg"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Alg { get; set; }

        /// <summary>Permitted public operations, e.g. <c>verify</c>.</summary>
        [JsonPropertyName("key_ops"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public IList<string> KeyOps { get; set; }

        /// <summary>RSA modulus.</summary>
        [JsonPropertyName("n"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string N { get; set; }

        /// <summary>RSA public exponent.</summary>
        [JsonPropertyName("e"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string E { get; set; }

        /// <summary>EC curve.</summary>
        [JsonPropertyName("crv"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Crv { get; set; }

        /// <summary>EC x coordinate.</summary>
        [JsonPropertyName("x"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string X { get; set; }

        /// <summary>EC y coordinate.</summary>
        [JsonPropertyName("y"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Y { get; set; }

        /// <summary>X.509 certificate chain (public certificates).</summary>
        [JsonPropertyName("x5c"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public IList<string> X5c { get; set; }

        /// <summary>X.509 SHA-1 thumbprint.</summary>
        [JsonPropertyName("x5t"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string X5t { get; set; }

        /// <summary>X.509 SHA-256 thumbprint.</summary>
        [JsonPropertyName("x5t#S256"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string X5tS256 { get; set; }

        /// <summary>X.509 URL.</summary>
        [JsonPropertyName("x5u"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string X5u { get; set; }
    }
}
