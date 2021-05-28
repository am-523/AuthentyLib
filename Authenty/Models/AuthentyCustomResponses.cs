using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Models
{
    public class AuthentyCustomResponses
    {
        public string _errorCode;
        public int? _level = null;

        public string errorCode
        {
            get { return _errorCode; }
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
            get { return _level ?? 1; }
            set
            {
                _level = value;
            }
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

        public bool InvalidApplicationHash { get; set; } = false;
    }
}
