using System;

namespace Authenty.Exceptions
{
    public class ApplicationNotFoundException : Exception
    {
        public ApplicationNotFoundException() : base("This application was not found. Verify your Application Key/ID!")
        {
        }
    }
}