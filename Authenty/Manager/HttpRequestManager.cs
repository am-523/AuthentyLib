using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Authenty.Models.HTTPRequests;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Authenty.Manager
{
    public class HttpRequestManager : Globals
    {

        private readonly CookieContainer _cookieContainer;
        internal HTTPCommunicationRequests CommunicationModel = null;

        public HttpRequestManager()
        {
            _cookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Set the session private authorization keys
        /// </summary>
        /// <param name="communicationModel">Http Secure Keys</param>
        public void SetCommunicationEstablished(HTTPCommunicationRequests communicationModel)
        {
            CommunicationModel = communicationModel;
        }

        public async Task<HttpResponseMessage> PostAsync(
            Dictionary<string, string> formDataEncodedContent, Dictionary<string, string> headers = null)
        {
            try
            {
                using var handler = new HttpClientHandler()
                {
                    CookieContainer = _cookieContainer,
                    ServerCertificateCustomValidationCallback = pinPublicKey,
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls
                };
                
                using var httpClient = new HttpClient(handler) {BaseAddress = new Uri(ApiEndPoint)};

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/authenty/csharp/")
                {
                    Content = new FormUrlEncodedContent(formDataEncodedContent)
                };

                if (CommunicationModel != null)
                {
                    httpClient.DefaultRequestHeaders.Add("authorization-id", CommunicationModel.SecuredAuthorizationKey);
                    httpClient.DefaultRequestHeaders.Add("application-key", CommunicationModel.SecuredApplicationKey);
                }

                if (headers != null)
                {
                    foreach (var i in headers)
                        httpClient.DefaultRequestHeaders.Add(i.Key, i.Value);
                }

                return await httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException("An error occurred in the request, please check your internet connectivity. If this problem continues, contact a developer.");
            }
        }

        private bool pinPublicKey(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return cert != null && cert.GetPublicKeyString() == Globals.IdCertificacionKey;
        }
    }
}
