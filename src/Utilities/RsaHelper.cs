using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace LtiAdvantageLibrary.NetCore.Utilities
{
    //https://dejanstojanovic.net/aspnet/2018/june/loading-rsa-key-pair-from-pem-files-in-net-core-with-c/
    public static class RsaHelper
    {
        public static RSACryptoServiceProvider PrivateKeyFromPemString(string privateKey)  
        {  
            using (var privateKeyTextReader = new StringReader(privateKey))  
            {  
                var readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();  
  
                var privateKeyParams = ((RsaPrivateCrtKeyParameters)readKeyPair.Private);  
                var cryptoServiceProvider = new RSACryptoServiceProvider();  
                var parms = new RSAParameters();  
  
                parms.Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned();  
                parms.P = privateKeyParams.P.ToByteArrayUnsigned();  
                parms.Q = privateKeyParams.Q.ToByteArrayUnsigned();  
                parms.DP = privateKeyParams.DP.ToByteArrayUnsigned();  
                parms.DQ = privateKeyParams.DQ.ToByteArrayUnsigned();  
                parms.InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned();  
                parms.D = privateKeyParams.Exponent.ToByteArrayUnsigned();  
                parms.Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned();  
  
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
                var parms = new RSAParameters();

                parms.Modulus = publicKeyParam.Modulus.ToByteArrayUnsigned();
                parms.Exponent = publicKeyParam.Exponent.ToByteArrayUnsigned();

                cryptoServiceProvider.ImportParameters(parms);

                return cryptoServiceProvider;
            }
        }
    }
}
