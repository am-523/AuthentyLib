using System;

namespace Authenty.Models.Sessions
{
    public class RemoteVariable
    {
        private string _secretCode;

        public string SecretCode
        {
            get => _secretCode;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _secretCode = value;
            }
        }
    }
}
