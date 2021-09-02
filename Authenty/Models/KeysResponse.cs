using System;

namespace Authenty.Models
{
    public class KeysResponse
    {
        private string _errorCode;

        public string errorCode
        {
            get => _errorCode;
            set
            {
                _errorCode = value.Trim() switch
                {
                    "INVALID_SESSION_OR_APPLICATION" => throw new UnauthorizedAccessException(),
                    "EMPTY_FIELDS" => throw new ArgumentException(),
                    "INTERNAL_ERROR" => throw new SystemException(),
                    _ => value
                };
            }
        }
    }
}
