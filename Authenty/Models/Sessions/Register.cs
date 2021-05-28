using System;
using System.Collections.Generic;
using System.Text;

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
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _username = value;
            }
        }

        public string password
        {
            get { return _password; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _password = value;
            }
        }

        public string email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _email = value;
            }
        }

        public string license
        {
            get { return _license; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _license = value;
            }
        }
    }
}
