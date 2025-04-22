using System;

namespace Infuse.Common
{
    public class InfuseException : Exception
    {
        public InfuseException(string message) : base(message)
        {
            // ...
        }
    }
}
