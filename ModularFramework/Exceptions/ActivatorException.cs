using System;

namespace ModularFramework.Exceptions
{
    public class ActivatorException : Exception
    {
        public Type TryType { get; }

        public ActivatorException(Type tryType, Exception innerException)
            : base("", innerException)
        {
            TryType = tryType;
        }

        public override string Message => $"Failed to create an instance of the type {TryType.FullName}!";
    }
}
