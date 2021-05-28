using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models.Sessions
{
    public class RemoteVariable
    {
        private string _SecretCode;

        public string SecretCode
        {
            get { return _SecretCode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _SecretCode = value;
            }
        }
    }
}
