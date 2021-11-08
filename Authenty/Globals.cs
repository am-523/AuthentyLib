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
            "04ACA1C4D9BF0004FA91C24DCE74C8CAD1A49B880F2FAB2A022725E43DAAC7A347CA369F97CC5B04DAA4AA79AA539EE5F981395FF9AAAE09D86D7FEA40B9ECAC7E";

        private static readonly string _ApiUrl = "https://api.biitez.dev/";
    }
}
