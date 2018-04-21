using System;

namespace General
{
    /// <summary>
    /// Represents an error that is caused by an implementation failure
    /// </summary>
    public class ImplementationError : Error
    {
        public ImplementationError(string message, Exception innerException = null) : base (message, innerException)
        {
        }
    }
}