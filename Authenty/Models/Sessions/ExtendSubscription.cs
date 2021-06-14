using System;

namespace Authenty.Models.Sessions
{
    public class ExtendSubscription
    {
        private string _username;
        private string _password;
        private string _license;

        public string username 
        { 
            get => _username;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _username = value;
            }
        }

        public string password
        {
            get => _password;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _password = value;
            }
        }

        public string license
        {
            get => _license;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                _license = value;
            }
        }
    }
}
