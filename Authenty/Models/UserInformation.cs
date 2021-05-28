using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models
{
    public class UserInformation
    {
        private int? _Level = null;
        private string _Username;
        private string _Email;
        private string _ExpireDate;
        private string _HWID;

        public int? Level
        {
            get { return _Level ?? 1; } // default level = 1
            set
            {
                if (value != null)
                {
                    if (value < 1 || value > 10)
                        throw new ArgumentOutOfRangeException("The user level must be a number greater than 0 and equal to or less than 10.");

                    _Level = value;
                }
            }
        }

        public string Username
        {
            get { return _Username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _Username = value;
            }
        }

        public string Email
        {
            get { return _Email; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _Email = value;
            }
        }

        public string ExpireDate
        {
            get { return _ExpireDate; }
            set
            {
                _ExpireDate = value ?? "Lifetime";
            }
        }

        public string HWID
        {
            get { return _HWID; }
            set
            {
                _HWID = value;
            }
        }
    }
}
