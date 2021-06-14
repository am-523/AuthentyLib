using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Authentication;
using Authenty.Models;

namespace Authenty.Manager
{
    internal class AuthentyClient : Globals
    {
        private AuthorizationKeys _authKeys;
        internal bool Connected => _httpClient.DefaultRequestHeaders.Contains(_authKeys.PrivAuthKey.name);


        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy(),
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                cert != null && cert.GetPublicKeyString() == IdCertificacionKey
        })
        {
            BaseAddress = ApiUrl
        };

        public void SetCommunicationKeys(AuthorizationKeys authKeys)
        {
            _authKeys = authKeys;

            _httpClient.DefaultRequestHeaders.Add(authKeys.PrivAuthKey.name, authKeys.PrivAuthKey.key);
            _httpClient.DefaultRequestHeaders.Add(authKeys.CipherAppKey.name, authKeys.CipherAppKey.key);
        }

        public async Task<HttpResponseMessage> SendAsync(IDictionary<string, string> formData,
            IDictionary<string, string> headers = null)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/authenty/csharp/")
                {
                    Content = new FormUrlEncodedContent(formData)
                };

                if (headers == null) return await _httpClient.SendAsync(httpRequestMessage);

                foreach (var i in headers)
                    _httpClient.DefaultRequestHeaders.Add(i.Key, i.Value);

                return await _httpClient.SendAsync(httpRequestMessage);
            }
            catch
            {
                throw new HttpRequestException(
                    "An error occurred in the request, please check your internet connectivity. If this problem continues, contact a developer.");
            }
        }
    }
}