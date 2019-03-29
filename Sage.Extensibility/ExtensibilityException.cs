using System;

namespace Sage.Extensibility
{
    public class ExtensibilityException : Exception
    {
        public ExtensibilityException() { }

        public ExtensibilityException(string message)
            : base(message) { }

        public ExtensibilityException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
