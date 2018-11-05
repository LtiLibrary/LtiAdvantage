using System.IO;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace LtiAdvantageLibrary.NetCore.Utilities
{
    /// <summary>
    /// Helper utilities to read PEM formatted keys.
    /// <remarks>
    /// From https://dejanstojanovic.net/aspnet/2018/june/loading-rsa-key-pair-from-pem-files-in-net-core-with-c/
    /// </remarks>
    /// </summary>
    public static class RsaHelper
    {
        public class RsaKeyPair
        {
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }

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

        public static RSACryptoServiceProvider PrivateKeyFromPemString(string privateKey)  
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
  
                return cryptoServiceProvider;  
            }  
        }

        public static RSACryptoServiceProvider PublicKeyFromPemString(string publicKey)
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

                return cryptoServiceProvider;
            }
        }

        public static JsonWebKey PrivateJsonWebKeyFromPemString(string privateKey)
        {
            using (var keyTextReader = new StringReader(privateKey))
            {
                var cipherKeyPair = (AsymmetricCipherKeyPair)new PemReader(keyTextReader).ReadObject();  
  
                var keyParameters = (RsaPrivateCrtKeyParameters)cipherKeyPair.Private;  

                var jwk = new JsonWebKey
                {
                    Kty = JsonWebAlgorithmsKeyTypes.RSA,
                    N = Base64UrlEncoder.Encode(keyParameters.Modulus.ToByteArrayUnsigned()),
                    E = Base64UrlEncoder.Encode(keyParameters.PublicExponent.ToByteArrayUnsigned()),
                    Alg = "RS256",
                    Use = JsonWebKeyUseNames.Sig,
                    P = Base64UrlEncoder.Encode(keyParameters.P.ToByteArrayUnsigned()),
                    Q = Base64UrlEncoder.Encode(keyParameters.Q.ToByteArrayUnsigned()),
                    DP = Base64UrlEncoder.Encode(keyParameters.DP.ToByteArrayUnsigned()),
                    DQ = Base64UrlEncoder.Encode(keyParameters.DQ.ToByteArrayUnsigned()),
                    QI = Base64UrlEncoder.Encode(keyParameters.QInv.ToByteArrayUnsigned()),
                    D = Base64UrlEncoder.Encode(keyParameters.Exponent.ToByteArrayUnsigned())
                };
                return jwk;
            }
        }

        public static JsonWebKey PublicJsonWebKeyFromPemString(string publicKey)
        {
            using (var keyTextReader = new StringReader(publicKey))
            {
                var keyParameters = (RsaKeyParameters)new PemReader(keyTextReader).ReadObject();

                var jwk = new JsonWebKey
                {
                    Kty = JsonWebAlgorithmsKeyTypes.RSA,
                    N = Base64UrlEncoder.Encode(keyParameters.Modulus.ToByteArrayUnsigned()),
                    E = Base64UrlEncoder.Encode(keyParameters.Exponent.ToByteArrayUnsigned()),
                    Alg = "RS256",
                    Use = JsonWebKeyUseNames.Sig
                };
                return jwk;
            }
        }
    }
}
