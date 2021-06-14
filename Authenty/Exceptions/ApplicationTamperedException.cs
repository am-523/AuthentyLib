using System;

namespace Authenty.Exceptions
{
    public class ApplicationTamperedException : Exception
    {
        public ApplicationTamperedException() : base(
            "The integrity of this application could not be verified. (manipulated file)")
        {
        }
    }
}