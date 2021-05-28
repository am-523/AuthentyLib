using Authenty.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Authenty.Services
{
    public class AesCryptography
    {
        internal byte[] EncryptionKey { get; }
        internal byte[] EncryptionIv { get; }

        public AesCryptography()
        {
            EncryptionKey = new byte[256 / 8];
            EncryptionIv = new byte[128 / 8];

            using (var rnd = new RNGCryptoServiceProvider())
            {
                rnd.GetBytes(EncryptionKey);
                rnd.GetBytes(EncryptionIv);
            }
        }

        internal string Encrypt(string plainText)
        {
            try
            {
                var aes = new RijndaelManaged
                {
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC,
                    KeySize = 256,
                    Key = EncryptionKey,
                    IV = EncryptionIv
                };

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var msEncrypt = new MemoryStream();
                var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                var swEncrypt = new StreamWriter(csEncrypt);

                swEncrypt.Write(plainText);

                swEncrypt.Close();
                csEncrypt.Close();
                aes.Clear();

                return BaseConverters.ToUrlSafeBase64(msEncrypt.ToArray());
            }
            catch
            {
                throw new CryptographicException("A problem occurred when trying to encrypt the message to the server, if this continues, contact a support!");
            }
        }

        internal string Decrypt(string cipherText)
        {
            try
            {
                var aes = new RijndaelManaged
                {
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC,
                    KeySize = 256,
                    Key = EncryptionKey,
                    IV = EncryptionIv
                };

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                var msDecrypt = new MemoryStream(BaseConverters.FromUrlSafeBase64(cipherText));
                var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                var srDecrypt = new StreamReader(csDecrypt);

                var plaintext = srDecrypt.ReadToEnd();

                srDecrypt.Close();
                csDecrypt.Close();
                msDecrypt.Close();
                aes.Clear();

                return plaintext;
            }
            catch
            {
                throw new CryptographicException("There was a problem decrypting the message from the server, if this continues, contact a support!");
            }
        }
    }
}
