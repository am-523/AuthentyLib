using System;

namespace Authenty.Exceptions
{
    public class PausedApplicationException : Exception
    {
        public PausedApplicationException() : base(
            "This application is currently paused. (Possibly for some maintenance or similar) ~ for more information, contact a support.")
        {
        }
    }
}