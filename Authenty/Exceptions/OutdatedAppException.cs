using Authenty.Helpers;
using System;

namespace Authenty.Exceptions
{
    public class OutdatedAppException : Exception
    {
        public OutdatedAppException(string urlUpdater) : base(
            "The current version of this application is out of date, please download the new one from the link to which you will be redirected. If you get an error downloading it, contact a developer.")
        {
            UriHelper.Open(urlUpdater);
        }
    }
}