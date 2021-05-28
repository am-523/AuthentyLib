using Authenty.Models;
using Authenty.Services;
using Authenty.Manager;
using Authenty.Services.Cryptographies;
using System;
using System.Net;
using System.Collections.Generic;
using Authenty.Helpers;
using Newtonsoft.Json;
using Authenty.Exceptions;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Authenty
{
    /// <summary>
    /// 
    ///                                                   _____________________
    /// _________________________________________________/                    |
    /// |                                                | .NET Standard 2.0  |
    /// |   Authenty.ME ~ Official .NET Library v1.0.0   |   Compatibilities  |
    /// |________________________________________________|____________________|
    /// |                        |                                            |
    /// |     Implementation     |                 Version                    |
    /// |________________________|____________________________________________|
    /// |                        |                                            |
    /// | .NET Core & .NET 5.0   | 2.0 - 2.1 - 2.2 - 3.0 - 3.1 - 5.0          |
    /// | .NET Framework         | 4.6.1 - 4.6.2 - 4.7 - 4.7.1 - 4.7.2 - 4.8  |
    /// |  Mono                  | 5.4 - 6.4                                  |
    /// |  Xamarin.iOS           | 10.14 - 12.16                              |
    /// |  Xamarin.Android       | 8.0 - 10.0                                 |
    /// |  Uni. Windows Platform | 10.0.16299 - TBD                           |
    /// |  Unity                 | 2018.1                                     |
    /// |________________________|____________________________________________|
    ///       \                                                          /
    ///        \  © 20-21 . Biitez's Space ~ https://github.com/biitez  /
    ///         \______________________________________________________/
    ///                 
    /// </summary>
    public class Licensing
    {
        private bool CommunicationEstablished => _httpRequestManager.CommunicationModel != null;
        private Dictionary<string, string> _remoteVariables = new Dictionary<string, string>();

        private readonly Configuration _applicationConfig;
        private RsaCryptography _rsaCryptography;
        private AesCryptography _aesCryptography;
        private HttpRequestManager _httpRequestManager;
        internal UserInformation UserInformation;

        /// <summary>
        /// Initialize the instance
        /// </summary>
        /// <param name="applicationConfig">Information of your application collected in the panel.</param>
        public Licensing(Configuration applicationConfig)
        {            
            _applicationConfig = applicationConfig;            
        }

        /// <summary>
        /// Connect to the server and create a session using the information from your application.
        /// </summary>
        /// <returns></returns>
        public Licensing Connect()
        {
            WebRequest.DefaultWebProxy = new WebProxy();

            _rsaCryptography = new RsaCryptography(
                new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    Convert.FromBase64String(_applicationConfig.RsaPubKey)));

            _aesCryptography = new AesCryptography();
            _httpRequestManager = new HttpRequestManager();

            var hwid = new UIDManager().Id;

            var cryptoSessionInfo = new Dictionary<string, string>()
            {
                { "session_key", BaseConverters.ToUrlSafeBase64(_rsaCryptography.Encrypt(_aesCryptography.EncryptionKey)) },
                { "session_iv", BaseConverters.ToUrlSafeBase64(_rsaCryptography.Encrypt(_aesCryptography.EncryptionIv)) },
            };

            var appConnectionHeaders = new Dictionary<string, string>()
            {
                { "Application-Version", _applicationConfig.ApplicationVersion },
                { "Application-MD5", _aesCryptography.Encrypt(Md5Hash.CalculateFile(Assembly.GetEntryAssembly().Location)) },
                { "Application-ID", _applicationConfig.ApplicationId.ToString() },
                { "HWID-PC", hwid }
            };

            var connectionResponse = _httpRequestManager.PostAsync(cryptoSessionInfo, appConnectionHeaders).Result;

            if (!connectionResponse.IsSuccessStatusCode)
            {
                throw connectionResponse.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new AccessDeniedException(),
                    HttpStatusCode.Forbidden => new BlackListedApplicationException(),
                    HttpStatusCode.NotFound => new ApplicationNotFoundException(),
                    _ => new Exception("An unexpected error occurred, please check your internet connection. If this error persists, contact a developer.")
                };
            }

            var connectionJsonResponse = JsonConvert.DeserializeObject<AuthentyCustomResponses>(
                _aesCryptography.Decrypt(connectionResponse.Content.ReadAsStringAsync().Result));

            switch (connectionJsonResponse)
            {
                case {success: false}: throw new Exception("An unexpected error has occurred, contact support if this continues.");
                case {ApplicationEnabled: false}: throw new PausedApplicationException();

                default:
                    
                    if (connectionJsonResponse != null)
                        _httpRequestManager.SetCommunicationEstablished(new Models.HTTPRequests.HTTPCommunicationRequests()
                        {
                            SecuredApplicationKey = _aesCryptography.Encrypt(_applicationConfig.ApplicationKey),
                            SecuredAuthorizationKey = connectionJsonResponse.authorizationKey
                        });

                    return this;
            }
        }

        /// <summary>
        /// Login to your application, a private session 
        /// will be created if the credentials are correct.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public (bool, string) Login(string username, string password)
        {
            if (!CommunicationEstablished)
                throw new AuthentyConnectionFailedException();

            var loginParamsInfo = new Dictionary<string, string>
            {
                {
                    "data",
                    _aesCryptography.Encrypt(
                        JsonConvert.SerializeObject(new Models.Sessions.Login()
                        {
                            username = username, password = password
                        }))
                },
                {"type", "login"}
            };            

            var loginResponse =
                JsonConvert.DeserializeObject<AuthentyCustomResponses>(_aesCryptography.Decrypt(_httpRequestManager.PostAsync(loginParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result));

            if (loginResponse is {success: false})
            {
                return (false, loginResponse.errorCode switch
                {
                    "INVALID_USERNAME_OR_PASSWORD" => "Invalid username/password",
                    "INVALID_HWID" => "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "EXPIRED_SUBSCRIPTION" => "Your account subscription has ended!",
                    "USER_BANNED" => $"You are banned from this application. Reason: {loginResponse.bannedReason}",
                    _ => throw new InvalidOperationException()
                });
            }

            UserInformation = new UserInformation
            {
                Email = loginResponse.email,
                Username = loginResponse.username,
                ExpireDate = loginResponse.expiredate,
                Level = loginResponse.level,
                HWID = loginResponse.hwid
            };

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(loginResponse.updaterVersion) && !string.IsNullOrEmpty(loginResponse.updaterLink))
                throw new OutdatedAppException(loginResponse.updaterLink);
            else if (loginResponse.InvalidApplicationHash) // Application Hash Checker API-Based
                throw new UnauthorizedAccessException("The hash of this application is different from the original, if this is an error, contact a developer.");

            return (true, "You have successfully logged in!");
        }
 

        /// <summary>
        /// At the moment when a user registers, a session is created just as he would 
        /// when logging in, so he remains with the session started, you can optionally 
        /// make the registered user be redirected to the menu of his application or that 
        /// have to log in again depending on your needs.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="license">License</param>
        /// <returns></returns>
        public (bool Success, string Message) Register(string username, string password, string email, string license)
        {
            if (!CommunicationEstablished)
                throw new AuthentyConnectionFailedException();

            var registerParamsInfo = new Dictionary<string, string>
            {
                {
                    "data",
                    _aesCryptography.Encrypt(JsonConvert.SerializeObject(new Models.Sessions.Register()
                    {
                        username = username, password = password, email = email, license = license
                    }))
                },
                {"type", "register"}
            };

            var registerResponse =
                JsonConvert.DeserializeObject<AuthentyCustomResponses>(_aesCryptography.Decrypt(_httpRequestManager.PostAsync(registerParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result));

            if (registerResponse is {success: false})
            {
                return (false, registerResponse.errorCode switch
                {
                    "ALREADY_USED_USERNAME_OR_EMAIL" => "The username and/or email is already used.",
                    "ALREADY_USED" => "The license entered has already been used.",
                    "INVALID_LICENSE" => "The entered license is invalid!",
                    _ => throw new InvalidOperationException()
                });
            }

            UserInformation = new UserInformation
            {
                Email = registerResponse.email,
                Username = registerResponse.username,
                ExpireDate = registerResponse.expiredate,
                Level = registerResponse.level,
                HWID = registerResponse.hwid
            };

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(registerResponse.updaterVersion) && !string.IsNullOrEmpty(registerResponse.updaterLink))
                throw new OutdatedAppException(registerResponse.updaterLink);
            else if (registerResponse.InvalidApplicationHash) // Application Hash Checker API-Based
                throw new UnauthorizedAccessException("The hash of this application is different from the original, if this is an error, contact a developer.");

            return (true, "You have been registered successfully!");
        }

        /// <summary>
        /// Login only using the license key as parameter
        /// </summary>
        /// <param name="license">License</param>
        /// <returns></returns>
        public (bool Success, string Message) LicenseLogin(string license)
        {
            if (!CommunicationEstablished)
                throw new AuthentyConnectionFailedException();

            var licenseLoginParamsInfo = new Dictionary<string, string>
            {
                {
                    "data",
                    _aesCryptography.Encrypt(
                        JsonConvert.SerializeObject(new Models.Sessions.LicenseLogin() {license = license}))
                },
                {"type", "licenseLogin"}
            };

            var licenseLoginResponse =
                JsonConvert.DeserializeObject<AuthentyCustomResponses>(
                    _aesCryptography.Decrypt(_httpRequestManager.PostAsync(licenseLoginParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result));

            if (licenseLoginResponse is {success: false})
            {
                return (false, licenseLoginResponse.errorCode switch
                {
                    "INVALID_HWID" => "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "EXPIRED_SUBSCRIPTION" => "Your account subscription has ended!",
                    "INVALID_LICENSE" => "The entered license is invalid!",
                    "USER_BANNED" => string.Format("You are banned from this application. Reason: {0}", licenseLoginResponse.bannedReason),
                    _ => throw new InvalidOperationException()
                });
            }

            UserInformation = new UserInformation
            {
                Email = licenseLoginResponse.email,
                Username = licenseLoginResponse.username,
                ExpireDate = licenseLoginResponse.expiredate,
                Level = licenseLoginResponse.level,
                HWID = licenseLoginResponse.hwid
            };

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(licenseLoginResponse.updaterVersion) && !string.IsNullOrEmpty(licenseLoginResponse.updaterLink))
                throw new OutdatedAppException(licenseLoginResponse.updaterLink);
            else if (licenseLoginResponse.InvalidApplicationHash) // Application Hash Checker API-Based
                throw new UnauthorizedAccessException("The hash of this application is different from the original, if this is an error, contact a developer.");

            return (true, "You have successfully logged in!");
        }

        /// <summary>
        /// Will add the expiration time of the license that you place in the time of the user's account
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="license">License-Key (The expiration time of this license will be set to the user)</param>
        /// <returns></returns>
        public (bool Success, string Message) ExtendSubscription(string username, string password, string license)
        {
            if (!CommunicationEstablished)
                throw new AuthentyConnectionFailedException();

            var extendSubscriptionParams = new Dictionary<string, string>
            {
                {
                    "data", _aesCryptography.Encrypt(JsonConvert.SerializeObject(
                        new Models.Sessions.ExtendSubscription()
                        {
                            username = username, password = password, license = license
                        }))
                },
                {"type", "extendSubscription"}
            };


            var extendSubscriptionServerResponse = _aesCryptography.Decrypt(
                _httpRequestManager.PostAsync(extendSubscriptionParams).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result);

            var extendSubscriptionResponse =
                JsonConvert.DeserializeObject<AuthentyCustomResponses>(extendSubscriptionServerResponse);

            if (extendSubscriptionResponse is {success: false})
            {
                return (false, extendSubscriptionResponse.errorCode switch
                {
                    "INVALID_USERNAME_OR_PASSWORD" => "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "USER_BANNED" => string.Format("You are banned from this application. Reason: {0}", extendSubscriptionResponse.bannedReason),
                    "INVALID_LICENSE" => "The entered license is invalid!",
                    _ => throw new InvalidOperationException()
                });
            }

            return (true, "Your subscription has been extended! Please, re-login in the application.");
        }


        /// <summary>
        /// Get the value of a remote variable, this method can only be executed if the user is inside a session.
        /// </summary>
        /// <param name="variableCode">variable secret 10-digit code</param>
        /// <returns></returns>
        public (bool Success, string Message) GetVariable(string variableCode)
        {
            if (!CommunicationEstablished)
                throw new AuthentyConnectionFailedException();

            var remoteVariableParams = new Dictionary<string, string>
            {
                {
                    "data",
                    _aesCryptography.Encrypt(
                        JsonConvert.SerializeObject(new Models.Sessions.RemoteVariable() {SecretCode = variableCode}))
                },
                {"type", "variable"}
            };

            var remoteVariableResponse =
                JsonConvert.DeserializeObject<AuthentyCustomResponses>(
                    _aesCryptography.Decrypt(_httpRequestManager.PostAsync(remoteVariableParams).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result));

            if (remoteVariableResponse is {success: false})
            {
                return (false, remoteVariableResponse.errorCode switch
                {
                    "UNFOUND_VARIABLE" => throw new InvalidOperationException("Variable not found, closing application for security reasons ..."),
                    "NO_LOGGED" => "You need to be logged in to grab secure-remote variables from the server!",
                    _ => throw new InvalidOperationException()
                });
            }

            if (string.IsNullOrEmpty(remoteVariableResponse?.value))
                throw new ArgumentNullException();

            var variableValue = _remoteVariables.FirstOrDefault(vr => vr.Key.Contains(variableCode)).Value;

            return (true, variableValue switch
            {
                null => AddRemoteVariable(variableCode, remoteVariableResponse.value),
                _ => variableValue
            });
        }

        private string AddRemoteVariable(string code, string value)
        {
            _remoteVariables.Add(code, value);
            return value;
        }
    }
}
