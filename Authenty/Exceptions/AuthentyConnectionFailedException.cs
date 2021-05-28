using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Exceptions
{
    public class AuthentyConnectionFailedException : Exception
    {
        public AuthentyConnectionFailedException() : base("Failed to connect the application with the servers. (Verify that it is properly configured)")
        {
        }
    }
}
