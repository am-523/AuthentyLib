using System;

namespace Authenty.Exceptions
{
    public class UnhandledException : Exception
    {
        public UnhandledException(string message) : base(message)
        {
        }
    }
}
