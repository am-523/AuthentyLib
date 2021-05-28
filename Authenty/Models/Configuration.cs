using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models
{
    public class Configuration
    {

        private string _RsaPubKey;
        private string _ApplicationKey;
        private string _ApplicationVersion;
        private int? _ApplicationId;

        /// <summary>
        /// The RSA public key for your application can be found at https://biitez.dev/services/authenty/application/{ID-Key}/settings
        /// </summary>
        public string RsaPubKey
        {
            get { return _RsaPubKey; }
            set
            {
                if (value.Length != 1172)
                {
                    throw new FormatException("The length of your RSA Public Key is Invalid, contact the developer if this is an error.");
                }

                _RsaPubKey = value;
            }
        }

        /// <summary>
        /// Random MD5 Hash that identifies your application, you can find it in https://biitez.dev/services/authenty/
        /// </summary>
        public string ApplicationKey
        {
            get { return _ApplicationKey; }
            set
            {
                if (value.Length != 32)
                {
                    throw new ArgumentException("The length of your Application-Key is Invalid, contact the developer if this is an error.");
                }

                _ApplicationKey = value;
            }
        }

        /// <summary>
        /// The version of your application.
        /// It is only necessary if you are using the Auto-Updater
        /// </summary>
        public string ApplicationVersion
        {
            get { return _ApplicationVersion; }
            set
            {               
                _ApplicationVersion = value ?? throw new ArgumentNullException("You must assign a value to the version! (if you don't want to integrate it, don't try to put a value on it, just don't)");
            }
        }

        /// <summary>
        /// The ID of your application, with 7 num characters, can be found in https://biitez.dev/services/authenty/
        /// </summary>
        public int? ApplicationId
        {
            get { return _ApplicationId; }
            set
            {
                _ApplicationId = value ?? throw new ArgumentNullException();
            }
        }
    }
}
