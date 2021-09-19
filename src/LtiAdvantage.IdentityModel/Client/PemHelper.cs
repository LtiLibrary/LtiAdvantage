using System.IO;
using System.Security.Cryptography;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// Helper utilities to read PEM formatted keys.
    /// <remarks>
    /// From https://dejanstojanovic.net/aspnet/2018/june/loading-rsa-key-pair-from-pem-files-in-net-core-with-c/
    /// </remarks>
    /// </summary>
    public static class PemHelper
    {
        /// <summary>
        /// A private/public key pair as PEM formatted strings.
        /// </summary>
        public class RsaKeyPair
        {
            public RsaKeyPair()
            {
                KeyId = CryptoRandom.CreateRandomKeyString(8);
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
            var rsaGenerator = new RsaKeyPairGenerator();
            rsaGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            var keyPair = rsaGenerator.GenerateKeyPair();

            var rsaKeyPair = new RsaKeyPair();

            using (var privateKeyTextWriter = new StringWriter())
            {
                var pemWriter = new PemWriter(privateKeyTextWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();

                rsaKeyPair.PrivateKey = privateKeyTextWriter.ToString();
            }

            using (var publicKeyTextWriter = new StringWriter())
            {
                var pemWriter = new PemWriter(publicKeyTextWriter);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();

                rsaKeyPair.PublicKey = publicKeyTextWriter.ToString();
            }

            return rsaKeyPair;
        }

        /// <summary>
        /// Converts a private key in PEM format into an <see cref="RsaSecurityKey"/>.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <returns>The private key as an <see cref="RsaSecurityKey"/>.</returns>
        public static SigningCredentials SigningCredentialsFromPemString(string privateKey)
        {
            using (var keyTextReader = new StringReader(privateKey))
            {
                var cipherKeyPair = (AsymmetricCipherKeyPair)new PemReader(keyTextReader).ReadObject();

                var keyParameters = (RsaPrivateCrtKeyParameters)cipherKeyPair.Private;
                var parameters = new RSAParameters
                {
                    Modulus = keyParameters.Modulus.ToByteArrayUnsigned(),
                    P = keyParameters.P.ToByteArrayUnsigned(),
                    Q = keyParameters.Q.ToByteArrayUnsigned(),
                    DP = keyParameters.DP.ToByteArrayUnsigned(),
                    DQ = keyParameters.DQ.ToByteArrayUnsigned(),
                    InverseQ = keyParameters.QInv.ToByteArrayUnsigned(),
                    D = keyParameters.Exponent.ToByteArrayUnsigned(),
                    Exponent = keyParameters.PublicExponent.ToByteArrayUnsigned()
                };
                var key = new RsaSecurityKey(parameters);
                return new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            }
        }

        /// <summary>
        /// Converts a public key in PEM format into an <see cref="RsaSecurityKey"/>.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The public key as an <see cref="RsaSecurityKey"/>.</returns>
        public static RsaSecurityKey PublicKeyFromPemString(string publicKey)
        {
            using (var keyTextReader = new StringReader(publicKey))
            {
                var keyParameters = (RsaKeyParameters)new PemReader(keyTextReader).ReadObject();
                var parameters = new RSAParameters
                {
                    Modulus = keyParameters.Modulus.ToByteArrayUnsigned(),
                    Exponent = keyParameters.Exponent.ToByteArrayUnsigned()
                };
                return new RsaSecurityKey(parameters);
            }
        }
    }
}
