using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() : base("Our filters denied your access.") 
        { 
        }
    }
}
