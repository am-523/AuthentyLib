using System;

namespace Authenty.Models.Sessions
{
    public class LicenseLogin
    {
        private string _license;

        public string license
        {
            get => _license;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("You cannot leave empty spaces!");

                _license = value;
            }
        }
    }
}
