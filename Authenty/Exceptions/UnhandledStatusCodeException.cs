using System;
using System.Net;

namespace Authenty.Exceptions
{
    public class UnhandledStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public UnhandledStatusCodeException(HttpStatusCode status_code)
        {
            StatusCode = status_code;
        }
    }
}
