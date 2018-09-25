using System.IO;
using System.Security.Cryptography;
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
            using (var privateKeyTextReader = new StringReader(privateKey))  
            {  
                var readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();  
  
                var privateKeyParams = (RsaPrivateCrtKeyParameters)readKeyPair.Private;  
                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var parms = new RSAParameters
                {
                    Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                    P = privateKeyParams.P.ToByteArrayUnsigned(),
                    Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                    DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                    DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                    InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned(),
                    D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                    Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned()
                };
                cryptoServiceProvider.ImportParameters(parms);  
  
                return cryptoServiceProvider;  
            }  
        }

        public static RSACryptoServiceProvider PublicKeyFromPemString(string publicKey)
        {
            using (var publicKeyTextReader = new StringReader(publicKey))
            {
                var publicKeyParam = (RsaKeyParameters)new PemReader(publicKeyTextReader).ReadObject();

                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var parms = new RSAParameters
                {
                    Modulus = publicKeyParam.Modulus.ToByteArrayUnsigned(),
                    Exponent = publicKeyParam.Exponent.ToByteArrayUnsigned()
                };
                cryptoServiceProvider.ImportParameters(parms);

                return cryptoServiceProvider;
            }
        }
    }
}
