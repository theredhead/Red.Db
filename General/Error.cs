using System;

namespace General
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an easily localizable error that is intended to be propagated to a UI
    /// </summary>
    public class Error : Exception
    {
        public Error(string message, Exception innerException = null) : base (message, innerException)
        {
        }
    }
}