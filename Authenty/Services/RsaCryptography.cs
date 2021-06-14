using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Authenty.Services
{
    public class RsaCryptography
    {
        private readonly X509Certificate2 _rsaPubKey;

        public RsaCryptography(X509Certificate2 rsaPubKey) 
        {
            _rsaPubKey = rsaPubKey;
        }

        public byte[] Encrypt(byte[] message)
        {
            var publicprovider = (RSA)_rsaPubKey.PublicKey.Key;
            return publicprovider.Encrypt(message, RSAEncryptionPadding.Pkcs1);
        }
    }
}
