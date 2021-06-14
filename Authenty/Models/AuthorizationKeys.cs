using System;

namespace Authenty.Models
{
    /// <summary>
    /// Protecting your application against any threat, the requests that 
    /// are made after connecting will be integrated into the Headers ; More information below.
    /// </summary>
    public class AuthorizationKeys
    {
        private (string, string) _privAuthKey;

        /// <summary>
        /// After the user connects to our servers, a session is created under a private key 
        /// where the user's actions are carefully monitored by our bot. After several hours, 
        /// this key is cleaned from your application.
        /// </summary>
        public (string name, string key) PrivAuthKey
        {
            get => _privAuthKey;
            set
            {
                if (value.key.Length != 25)
                    throw new ApplicationException();

                _privAuthKey = (value.name, value.key);
            }
        }

        /// <summary>
        /// After connecting, the key of your application is sent through the Headers using 
        /// a high-power standard encryption, to guarantee the security of your application.
        /// </summary>
        public (string name, string key) CipherAppKey { get; set; }
    }
}