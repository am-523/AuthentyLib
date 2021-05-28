using Authenty.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Exceptions
{
    public class OutdatedAppException : Exception
    {
        public OutdatedAppException(string UrlUpdater) : base("The current version of this application is out of date, please download the new one from the link to which you will be redirected. If you get an error downloading it, contact a developer.")
        {
            UriHelper.Open(UrlUpdater);
        }
    }
}
