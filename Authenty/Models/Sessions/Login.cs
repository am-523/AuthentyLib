using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models.Sessions
{
    public class Login
    {
        private string _username;
        private string _password;

        public string username
        {
            get { return _username; }
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
            get { return _password; }
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
