using System;
using System.Collections.Generic;
using System.Text;

namespace Authenty.Exceptions
{
    public class UnderMaintenanceException : Exception
    {
        public UnderMaintenanceException() : base("Hey, our platform is currently under maintenance, please try again in a few minutes!")
        {
        }
    }
}
