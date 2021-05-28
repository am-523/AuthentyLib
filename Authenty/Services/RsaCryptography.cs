using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Authenty.Services.Cryptographies
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
