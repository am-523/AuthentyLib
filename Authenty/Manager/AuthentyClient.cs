using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Authentication;
using Authenty.Models;
using Authenty.Exceptions;
using System;

namespace Authenty.Manager
{
    internal class AuthentyClient : Globals
    {
        private AuthorizationKeys _authKeys;

        internal bool IsConnected()
        {
            return !_httpClient.DefaultRequestHeaders.Contains(_authKeys.SessionId?.name)
                ? throw new AuthentyConnectionFailedException()
                : true;
        }
        
        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy(),
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                cert != null && cert.GetPublicKeyString() == IdCertificacionKey
        }, false)
        {
            BaseAddress = ApiUrl
        };

        public void SetCommunicationKeys(AuthorizationKeys authKeys)
        {
            _authKeys = authKeys;

            _httpClient.DefaultRequestHeaders.Add(authKeys.SessionId?.name, authKeys.SessionId?.key);
            _httpClient.DefaultRequestHeaders.Add(authKeys.CipherAppKey.name, authKeys.CipherAppKey.key);
        }

        public async Task<HttpResponseMessage> SendAsync(IDictionary<string, string> formData,
            IDictionary<string, string> headers = null)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/authenty/csharp/v2/")
                {
                    Content = new FormUrlEncodedContent(formData)
                };

                if (headers != null)
                    foreach (var i in headers)
                        _httpClient.DefaultRequestHeaders.Add(i.Key, i.Value);

                var httpResponse = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.NotAcceptable: throw new ApplicationTamperedException();
                        case HttpStatusCode.ServiceUnavailable: throw new UnderMaintenanceException();
                    };
                }

                return httpResponse;
            }
            catch
            {
                throw new HttpRequestException(
                    "An error occurred in the request, please check your internet connectivity. If this problem continues, contact a developer.");
            }
        }
    }
}