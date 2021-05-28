using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Exceptions
{
    public class BlackListedApplicationException : Exception
    {
        public BlackListedApplicationException() : base("This app has been blacklisted by Authenty-ME for violating our agreement. (ToS)")
        {
        }
    }
}
