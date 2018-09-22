using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

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
