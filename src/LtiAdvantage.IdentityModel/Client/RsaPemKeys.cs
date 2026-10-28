using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// Creates and reads PEM formatted RSA keys, as a tool or platform stores them, using .NET's
    /// own PEM support.
    /// </summary>
    /// <remarks>
    /// Reads PKCS#1 (<c>BEGIN RSA PRIVATE KEY</c>), PKCS#8 (<c>BEGIN PRIVATE KEY</c>) and
    /// SubjectPublicKeyInfo (<c>BEGIN PUBLIC KEY</c>) keys. Generated private keys are PKCS#1,
    /// the format the IMS reference implementation and earlier versions of this library use.
    /// </remarks>
    public static class RsaPemKeys
    {
        /// <summary>
        /// Generates a new 2048-bit RSA key pair with a random key id.
        /// </summary>
        public static RsaPemKeyPair GenerateKeyPair()
        {
            using var rsa = RSA.Create(2048);
            return new RsaPemKeyPair(NewKeyId(), rsa.ExportRSAPrivateKeyPem(), rsa.ExportSubjectPublicKeyInfoPem());
        }

        /// <summary>
        /// Creates RS256 signing credentials from a PEM private key.
        /// </summary>
        /// <param name="privateKeyPem">The private key.</param>
        /// <param name="keyId">
        /// The key id, written as the JWT header's <c>kid</c>. Pass the id the key is published
        /// under in the JWKS, so the receiver can pick the right key after a rotation.
        /// </param>
        public static SigningCredentials SigningCredentialsFromPem(string privateKeyPem, string keyId)
        {
            var key = new RsaSecurityKey(ImportParameters(privateKeyPem, includePrivate: true)) {KeyId = keyId};
            return new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        }

        /// <summary>
        /// Reads a PEM public key (or the public half of a private key).
        /// </summary>
        /// <param name="publicKeyPem">The public key.</param>
        /// <param name="keyId">Optional key id to set on the key.</param>
        public static RsaSecurityKey PublicKeyFromPem(string publicKeyPem, string keyId = null) =>
            new RsaSecurityKey(ImportParameters(publicKeyPem, includePrivate: false)) {KeyId = keyId};

        internal static string NewKeyId() =>
            Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(8));

        private static RSAParameters ImportParameters(string pem, bool includePrivate)
        {
            if (string.IsNullOrWhiteSpace(pem)) throw new ArgumentException("A PEM key is required.", nameof(pem));

            using var rsa = RSA.Create();
            rsa.ImportFromPem(pem);
            // Throws when a private key is asked for but the PEM only holds a public one.
            return rsa.ExportParameters(includePrivate);
        }
    }

    /// <summary>
    /// An RSA key pair as PEM strings, with its key id.
    /// </summary>
    public class RsaPemKeyPair
    {
        /// <summary>Creates the key pair.</summary>
        public RsaPemKeyPair(string keyId, string privateKey, string publicKey)
        {
            KeyId = keyId;
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }

        /// <summary>The key id (<c>kid</c>).</summary>
        public string KeyId { get; }

        /// <summary>The private key (PKCS#1 PEM).</summary>
        public string PrivateKey { get; }

        /// <summary>The public key (SubjectPublicKeyInfo PEM).</summary>
        public string PublicKey { get; }
    }
}
