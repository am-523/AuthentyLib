using System;

namespace Authenty.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() : base("Our filters denied your access.")
        {
        }
    }
}