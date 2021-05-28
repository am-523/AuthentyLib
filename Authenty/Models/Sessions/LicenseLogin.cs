using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models.Sessions
{
    public class LicenseLogin
    {
        private string _license;

        public string license
        {
            get { return _license; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("You cannot leave empty spaces!");

                _license = value;
            }
        }
    }
}
