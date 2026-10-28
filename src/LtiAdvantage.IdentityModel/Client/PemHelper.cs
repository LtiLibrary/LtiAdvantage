using System;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// Helper utilities to read PEM formatted keys.
    /// </summary>
    /// <remarks>
    /// Kept for compatibility; use <see cref="RsaPemKeys"/>, which also sets the key id. The class
    /// name clashes with the <c>PemHelper</c> many apps have.
    /// </remarks>
    [Obsolete("Use RsaPemKeys, which sets the key id (kid) on signing credentials.")]
    public static class PemHelper
    {
        /// <summary>
        /// A private/public key pair as PEM formatted strings.
        /// </summary>
        public class RsaKeyPair
        {
            /// <summary>Creates an empty key pair with a random key id.</summary>
            public RsaKeyPair()
            {
                KeyId = Convert.ToBase64String(CryptoRandom.CreateRandomKey(8));
            }

            /// <summary>
            /// The KeyId for this key pair.
            /// </summary>
            public string KeyId { get; set; }

            /// <summary>
            /// The private key.
            /// </summary>
            public string PrivateKey { get; set; }

            /// <summary>
            /// The public key.
            /// </summary>
            public string PublicKey { get; set; }
        }

        /// <summary>
        /// Create a new private/public key pair as PEM formatted strings.
        /// </summary>
        /// <returns>An <see cref="RsaKeyPair"/>.</returns>
        public static RsaKeyPair GenerateRsaKeyPair()
        {
            var pair = RsaPemKeys.GenerateKeyPair();
            return new RsaKeyPair {PrivateKey = pair.PrivateKey, PublicKey = pair.PublicKey};
        }

        /// <summary>
        /// Converts a private key in PEM format into RS256 signing credentials, without a key id.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <returns>The signing credentials.</returns>
        public static SigningCredentials SigningCredentialsFromPemString(string privateKey) =>
            RsaPemKeys.SigningCredentialsFromPem(privateKey, null);

        /// <summary>
        /// Converts a private key in PEM format into RS256 signing credentials with a key id.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="keyId">The key id, written as the JWT header's <c>kid</c>.</param>
        /// <returns>The signing credentials.</returns>
        public static SigningCredentials SigningCredentialsFromPemString(string privateKey, string keyId) =>
            RsaPemKeys.SigningCredentialsFromPem(privateKey, keyId);

        /// <summary>
        /// Converts a public key in PEM format into an <see cref="RsaSecurityKey"/>.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The public key as an <see cref="RsaSecurityKey"/>.</returns>
        public static RsaSecurityKey PublicKeyFromPemString(string publicKey) =>
            RsaPemKeys.PublicKeyFromPem(publicKey);
    }
}
