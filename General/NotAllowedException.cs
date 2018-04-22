using System;

namespace General
{
    public class NotAllowedException : Error
    {
        public NotAllowedException(string message, Exception innerException=null) : base(message, innerException)
        {
        }
    }
}