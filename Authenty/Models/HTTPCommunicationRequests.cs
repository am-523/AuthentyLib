using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models.HTTPRequests
{
    /// <summary>
    /// Protecting your application against any threat, the requests that 
    /// are made after connecting will be integrated into the Headers ; More information below.
    /// </summary>
    public class HTTPCommunicationRequests
    {
        private string _SecuredAuthorizationKey;

        /// <summary>
        /// After the user connects to our servers, a session is created under a private key 
        /// where the user's actions are carefully monitored by our bot. After several hours, 
        /// this key is cleaned from your application.
        /// </summary>
        public string SecuredAuthorizationKey
        {
            get { return _SecuredAuthorizationKey; }
            set
            {
                if (value.Length != 25)
                    throw new ApplicationException();

                _SecuredAuthorizationKey = value;
            }
        }

        /// <summary>
        /// After connecting, the key of your application is sent through the Headers using 
        /// a high-power standard encryption, to guarantee the security of your application.
        /// </summary>
        public string SecuredApplicationKey { get; set; }
    }
}
