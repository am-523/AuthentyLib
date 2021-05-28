using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models.Sessions
{
    public class ExtendSubscription
    {
        private string _username;
        private string _password;
        private string _license;

        public string username 
        { 
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _username = value;
            }
        }

        public string password
        {
            get { return _password; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _password = value;
            }
        }

        public string license
        {
            get { return _license; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _license = value;
            }
        }
    }
}
