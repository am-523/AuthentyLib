using System;

namespace Authenty
{
    public class Globals
    {
        internal static Uri ApiUrl
        {
            get
            {
                if (!Uri.TryCreate(_ApiUrl, UriKind.Absolute, out var uriApi)
                    && (uriApi.Scheme == Uri.UriSchemeHttps))
                {
                    throw new UriFormatException("The API is invalid or does not work under HTTPS.");
                }

                return uriApi;
            }
        }

        internal const string IdCertificacionKey =
            "0457B6A98C2938730F3E0B10E0A1B6C897E3E3333DDA927B18F00D123A5C795346E6B7A972CA4B8EF97F8441D1281C283015AF6572B67148958062ED9708115D86";

        private static readonly string _ApiUrl = "https://api.biitez.dev/";
    }
}