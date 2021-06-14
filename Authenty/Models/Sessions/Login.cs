using System;

namespace Authenty.Models.Sessions
{
    public class Login
    {
        private string _username;
        private string _password;

        public string username
        {
            get => _username;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }

                _username = value;
            }
        }

        public string password
        {
            get => _password;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }

                _password = value;
            }
        }
    }
}
