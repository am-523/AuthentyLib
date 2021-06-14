using System;

namespace Authenty.Models.Sessions
{
    public class Register
    {
        private string _username;
        private string _password;
        private string _email;
        private string _license;

        public string username
        {
            get => _username;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _username = value;
            }
        }

        public string password
        {
            get => _password;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _password = value;
            }
        }

        public string email
        {
            get => _email;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _email = value;
            }
        }

        public string license
        {
            get => _license;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _license = value;
            }
        }
    }
}
