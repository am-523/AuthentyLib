using System;

namespace Authenty.Models
{
    public class AuthentyCustomResponses
    {
        private string _errorCode;
        private int? _level;

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

        public int? level
        {
            get => _level ?? 1;
            set => _level = value;
        }

        public bool success { get; set; } = false;
        public string authorizationKey { get; set; } = null;
        public bool ApplicationEnabled { get; set; } = false;
        public string bannedReason { get; set; } = null;        
        public string value { get; set; } = null;
        public string email { get; set; } = null;
        public string username { get; set; } = null;
        public string expiredate { get; set; } = null;        
        public string hwid { get; set; } = null;

        public string updaterVersion { get; set; } = null;
        public string updaterLink { get; set; } = null;
    }
}
