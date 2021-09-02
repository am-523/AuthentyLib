using Authenty.Models;
using Authenty.Services;
using Authenty.Manager;
using System;
using System.Net;
using System.Collections.Generic;
using Authenty.Helpers;
using Newtonsoft.Json;
using Authenty.Exceptions;
using Newtonsoft.Json.Linq;

namespace Authenty
{
    /// <summary>
    ///
    /// Compatibilities:
    ///  _____________________________________________________________________
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
    ///               Made with love by https://github.com/biitez
    ///                 
    /// </summary>
    public class Licensing
    {
        private IDictionary<string, string> _remoteVariables = new Dictionary<string, string>();

        private readonly Configuration _applicationConfig;
        private RsaCryptography _rsaCryptography;
        private AesCryptography _aesCryptography;
        private AuthentyClient _authentyClient;
        private KeysResponse _KeysResponse;

        public UserInformation UserInformation = UserInfo.Instance;

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
        public void Connect()
        {
            _rsaCryptography = new RsaCryptography(
                new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    Convert.FromBase64String(_applicationConfig.RsaPubKey)));

            _aesCryptography = new AesCryptography();
            _authentyClient = new AuthentyClient();

            var hwid = new UIDManager().Id;

            var cryptoSessionInfo = new Dictionary<string, string>()
            {
                {
                    "session_key",
                    BaseConverters.ToUrlSafeBase64(_rsaCryptography.Encrypt(_aesCryptography.EncryptionKey))
                },
                {"session_iv", BaseConverters.ToUrlSafeBase64(_rsaCryptography.Encrypt(_aesCryptography.EncryptionIv))},
            };

            var appConnectionHeaders = new Dictionary<string, string>()
            {
                {"Application-Version", _applicationConfig.ApplicationVersion},
                {"Application-MD5", _aesCryptography.Encrypt(Md5Hash.CurrentMd5File)},
                {"Application-ID", _applicationConfig.ApplicationId.ToString()},
                {"HWID-PC", hwid}
            };

            var HttpResponse = _authentyClient.SendAsync(cryptoSessionInfo, appConnectionHeaders).GetAwaiter()
                .GetResult();

            if (!HttpResponse.IsSuccessStatusCode)
            {
                throw HttpResponse.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new AccessDeniedException(),
                    HttpStatusCode.Forbidden => new BlackListedApplicationException(),
                    HttpStatusCode.NotFound => new ApplicationNotFoundException(),
                    _ => new UnhandledStatusCodeException(HttpResponse.StatusCode),
                };
            }

            var HttpResponseString = _aesCryptography.Decrypt(HttpResponse.Content.ReadAsStringAsync().Result);

            JToken _jsonElement = JObject.Parse(HttpResponseString);
            
            if (!(bool)_jsonElement.SelectToken("success"))
            {
                throw new Exception("An unexpected error has occurred, contact support if this continues.");
            }

            if (!(bool)_jsonElement.SelectToken("ApplicationEnabled"))
            {
                throw new PausedApplicationException();
            }

            string AuthorizationKey = (string)_jsonElement.SelectToken("authorizationKey") 
                ?? throw new NullReferenceException("Invalid Authorization Key");

            _authentyClient.SetCommunicationKeys(new AuthorizationKeys
            {
                SessionId = ("authorization-id", AuthorizationKey),
                CipherAppKey = ("application-key",
                    _aesCryptography.Encrypt(_applicationConfig.ApplicationKey)),
            });
        }

        /// <summary>
        /// Login to your application, a private session 
        /// will be created if the credentials are correct.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public (bool Success, string Message) Login(string username, string password)
        {
            _authentyClient.IsConnected();

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

            var LoginResponseString = _aesCryptography.Decrypt(_authentyClient
                    .SendAsync(loginParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result);

            JToken _jsonLoginElement = JObject.Parse(LoginResponseString);

            if (!(bool)_jsonLoginElement.SelectToken("success"))
            {
                _KeysResponse = new KeysResponse
                {
                    errorCode = (string)_jsonLoginElement.SelectToken("errorCode")
                };

                return (false, _KeysResponse.errorCode switch
                {
                    "INVALID_USERNAME_OR_PASSWORD" => "Invalid username/password",
                    "INVALID_HWID" =>
                        "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "EXPIRED_SUBSCRIPTION" => "Your account subscription has ended!",
                    "USER_BANNED" => $"You are banned from this application. Reason: {(string)_jsonLoginElement.SelectToken("bannedReason")}",
                    _ => throw new InvalidOperationException()
                });
            }

            string UserEmailObject = (string)_jsonLoginElement.SelectToken("email");
            string UserNameObject = (string)_jsonLoginElement.SelectToken("username");
            string UserExpireObject = (string)_jsonLoginElement.SelectToken("expiredate");
            int? UserLevelObject = (int?)_jsonLoginElement.SelectToken("level") ?? 1;
            string UserHwidObject = (string)_jsonLoginElement.SelectToken("hwid");

            UserInformation = new UserInformation
            {
                Email = UserEmailObject,
                Username = UserNameObject,
                ExpireDate = UserExpireObject,
                Level = UserLevelObject,
                Hwid = UserHwidObject
            };

            string updaterVersion = (string)_jsonLoginElement.SelectToken("updaterVersion");
            string updaterLink = (string)_jsonLoginElement.SelectToken("updaterLink");

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(updaterVersion) && !string.IsNullOrEmpty(updaterLink))
                throw new OutdatedAppException(updaterLink);

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
            _authentyClient.IsConnected();

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

            var RegisterResponseString = _aesCryptography.Decrypt(_authentyClient
                    .SendAsync(registerParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result);

            JToken _jsonRegisterElement = JObject.Parse(RegisterResponseString);

            //var _J =
            //    JsonConvert.DeserializeObject<AuthentyCustomResponses>(_aesCryptography.Decrypt(_authentyClient
            //        .SendAsync(registerParamsInfo).GetAwaiter().GetResult().Content.ReadAsStringAsync().Result));

            if (!(bool)_jsonRegisterElement.SelectToken("success"))
            {
                _KeysResponse.errorCode = (string)_jsonRegisterElement.SelectToken("errorCode");

                return (false, _KeysResponse.errorCode switch
                {
                    "ALREADY_USED_USERNAME_OR_EMAIL" => "The username and/or email is already used.",
                    "ALREADY_USED" => "The license entered has already been used.",
                    "INVALID_LICENSE" => "The entered license is invalid!",
                    _ => throw new InvalidOperationException()
                });
            }

            string UserEmailObject = (string)_jsonRegisterElement.SelectToken("email");
            string UserNameObject = (string)_jsonRegisterElement.SelectToken("username");
            string UserExpireObject = (string)_jsonRegisterElement.SelectToken("expiredate");
            int? UserLevelObject = (int?)_jsonRegisterElement.SelectToken("level") ?? 1;
            string UserHwidObject = (string)_jsonRegisterElement.SelectToken("hwid");

            UserInformation = new UserInformation
            {
                Email = UserEmailObject,
                Username = UserNameObject,
                ExpireDate = UserExpireObject,
                Level = UserLevelObject,
                Hwid = UserHwidObject
            };

            string updaterVersion = (string)_jsonRegisterElement.SelectToken("updaterVersion");
            string updaterLink = (string)_jsonRegisterElement.SelectToken("updaterLink");

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(updaterVersion) && !string.IsNullOrEmpty(updaterLink))
                throw new OutdatedAppException(updaterLink);

            return (true, "You have been registered successfully!");
        }

        /// <summary>
        /// Login only using the license key as parameter
        /// </summary>
        /// <param name="license">License</param>
        /// <returns></returns>
        public (bool Success, string Message) LicenseLogin(string license)
        {
            _authentyClient.IsConnected();

            var licenseLoginParamsInfo = new Dictionary<string, string>
            {
                {
                    "data",
                    _aesCryptography.Encrypt(
                        JsonConvert.SerializeObject(new Models.Sessions.LicenseLogin() {license = license}))
                },
                {"type", "licenseLogin"}
            };

            //var licenseLoginResponse =
            //    JsonConvert.DeserializeObject<AuthentyCustomResponses>(
            //        _aesCryptography.Decrypt(_authentyClient.SendAsync(licenseLoginParamsInfo).GetAwaiter().GetResult()
            //            .Content.ReadAsStringAsync().Result));
            
            var licenseLoginResponse =
                    _aesCryptography.Decrypt(_authentyClient.SendAsync(licenseLoginParamsInfo).GetAwaiter().GetResult()
                        .Content.ReadAsStringAsync().Result);

            JToken _jsonLicenseLoginElement = JObject.Parse(licenseLoginResponse);

            if (!(bool)_jsonLicenseLoginElement.SelectToken("success"))
            {
                _KeysResponse.errorCode = (string)_jsonLicenseLoginElement.SelectToken("errorCode");

                return (false, _KeysResponse.errorCode switch
                {
                    "INVALID_HWID" =>
                        "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "EXPIRED_SUBSCRIPTION" => "Your account subscription has ended!",
                    "INVALID_LICENSE" => "The entered license is invalid!",
                    "USER_BANNED" => string.Format("You are banned from this application. Reason: {0}",
                        (string)_jsonLicenseLoginElement.SelectToken("bannedReason")),
                    _ => throw new InvalidOperationException()
                });
            }

            string UserEmailObject = (string)_jsonLicenseLoginElement.SelectToken("email");
            string UserNameObject = (string)_jsonLicenseLoginElement.SelectToken("username");
            string UserExpireObject = (string)_jsonLicenseLoginElement.SelectToken("expiredate");
            int? UserLevelObject = (int?)_jsonLicenseLoginElement.SelectToken("level") ?? 1;
            string UserHwidObject = (string)_jsonLicenseLoginElement.SelectToken("hwid");

            UserInformation = new UserInformation
            {
                Email = UserEmailObject,
                Username = UserNameObject,
                ExpireDate = UserExpireObject,
                Level = UserLevelObject,
                Hwid = UserHwidObject
            };

            string updaterVersion = (string)_jsonLicenseLoginElement.SelectToken("updaterVersion");
            string updaterLink = (string)_jsonLicenseLoginElement.SelectToken("updaterLink");

            // AutoUpdater only works after the user logs in to prevent third-party downloads
            if (!string.IsNullOrEmpty(updaterVersion) && !string.IsNullOrEmpty(updaterLink))
                throw new OutdatedAppException(updaterLink);

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
            _authentyClient.IsConnected();

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
                _authentyClient.SendAsync(extendSubscriptionParams).GetAwaiter().GetResult().Content.ReadAsStringAsync()
                    .Result);

            JToken _jsonExtendSubElement = JObject.Parse(extendSubscriptionServerResponse);

            if (!(bool)_jsonExtendSubElement.SelectToken("success"))
            {
                _KeysResponse.errorCode = (string)_jsonExtendSubElement.SelectToken("errorCode");

                return (false, _KeysResponse.errorCode switch
                {
                    "INVALID_USERNAME_OR_PASSWORD" =>
                        "The HWID registered in this account is different. Contact support if you think this is a error.",
                    "USER_BANNED" =>
                        $"You are banned from this application. Reason: {(string)_jsonExtendSubElement.SelectToken("bannedReason")}",
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
            _authentyClient.IsConnected();

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
                    _aesCryptography.Decrypt(_authentyClient.SendAsync(remoteVariableParams).GetAwaiter().GetResult()
                        .Content.ReadAsStringAsync().Result);

            JToken _jsonVarResponseElement = JObject.Parse(remoteVariableResponse);

            if (!(bool)_jsonVarResponseElement.SelectToken("success"))
            {
                _KeysResponse.errorCode = (string)_jsonVarResponseElement.SelectToken("errorCode");

                return (false, _KeysResponse.errorCode switch
                {
                    "UNFOUND_VARIABLE" => throw new InvalidOperationException(
                        "The variable is not valid, please make sure to enter a correct ID."),
                    "NO_LOGGED" => "You need to be logged in to grab secure-remote variables from the server!",
                    _ => throw new InvalidOperationException()
                });
            }

            var RemoteVariable = (string)_jsonVarResponseElement.SelectToken("value") 
                ?? throw new ArgumentNullException("Invalid Variable");

            return (Success: true,
                _remoteVariables.TryGetValue(variableCode, out var value)
                    ? value
                    : AddRemoteVariable(variableCode, RemoteVariable));
        }

        private string AddRemoteVariable(string code, string value)
        {
            _remoteVariables.Add(code, value);
            return value;
        }
    }
}