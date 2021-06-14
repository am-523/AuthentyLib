using System;

namespace Authenty.Exceptions
{
    public class AuthentyConnectionFailedException : Exception
    {
        public AuthentyConnectionFailedException() : base(
            "Failed to connect the application with the servers. (Verify that it is properly configured)")
        {
        }
    }
}