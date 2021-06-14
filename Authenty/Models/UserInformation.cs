using System;

namespace Authenty.Models
{
    public static class UserInfo
    {
        private static readonly Lazy<UserInformation> Lazy = new Lazy<UserInformation>(()
            => new UserInformation());
        public static UserInformation Instance => Lazy.Value;
    }

    public class UserInformation
    {
        private int? _Level = null;
        private string _Username;
        private string _Email;
        private string _ExpireDate;
        private string _HWID;

        public int? Level
        {
            get => _Level ?? 1; // default level = 1
            set
            {
                if (value == null) return;
                if (value < 1 || value > 10)
                    throw new ArgumentOutOfRangeException(
                        "The user level must be a number greater than 0 and equal to or less than 10.");

                _Level = value;
            }
        }

        public string Username
        {
            get => _Username ?? throw new ArgumentNullException();
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _Username = value;
            }
        }

        public string Email
        {
            get => _Email ?? throw new ArgumentNullException();
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _Email = value;
            }
        }

        public string ExpireDate
        {
            get => _ExpireDate;
            set => _ExpireDate = value ?? "Lifetime";
        }

        public string Hwid
        {
            get => _HWID ?? null;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Invalid HWID Format");

                _HWID = value;
            }
        }
    }
}