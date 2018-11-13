using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace LtiAdvantageLibrary.Utilities
{
    /// <summary>
    /// Helper utilities to read PEM formatted keys.
    /// <remarks>
    /// From https://dejanstojanovic.net/aspnet/2018/june/loading-rsa-key-pair-from-pem-files-in-net-core-with-c/
    /// </remarks>
    /// </summary>
    public static class RsaHelper
    {
        /// <summary>
        /// A private/public key pair as PEM formatted strings.
        /// </summary>
        public class RsaKeyPair
        {
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
        /// Create a signed jwt.
        /// </summary>
        /// <param name="payload">The payload to include in the jwt.</param>
        /// <param name="privateKey">The private signing key in PEM format.</param>
        /// <returns>The signed jwt.</returns>
        public static string CreateSignedJwt(JwtPayload payload, string privateKey)
        {
            var key = RsaHelper.PrivateKeyFromPemString(privateKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var handler = new JsonWebTokenHandler();
            var jwt = handler.CreateToken(payload.SerializeToJson(), credentials);
            return jwt;
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
        private static RsaSecurityKey PrivateKeyFromPemString(string privateKey)  
        {  
            using (var keyTextReader = new StringReader(privateKey))  
            {  
                var cipherKeyPair = (AsymmetricCipherKeyPair)new PemReader(keyTextReader).ReadObject();  
  
                var keyParameters = (RsaPrivateCrtKeyParameters)cipherKeyPair.Private;  
                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var parms = new RSAParameters
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
                cryptoServiceProvider.ImportParameters(parms);  
  
                return new RsaSecurityKey(cryptoServiceProvider);  
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

                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var parms = new RSAParameters
                {
                    Modulus = keyParameters.Modulus.ToByteArrayUnsigned(),
                    Exponent = keyParameters.Exponent.ToByteArrayUnsigned()
                };
                cryptoServiceProvider.ImportParameters(parms);

                return new RsaSecurityKey(cryptoServiceProvider);
            }
        }
    }
}
