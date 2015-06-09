using System;
using FuwaTea.Lib.Exceptions;

namespace LayerFramework.Exceptions
{
    public class ActivatorException : Exception, IDepthElement
    {
        public Type TryType { get; private set; }

        public ActivatorException(Type tryType, Exception innerException)
            : base("", innerException)
        {
            TryType = tryType;
        }

        public override string Message
        {
            get { return string.Format("Failed to create an instance of the type {0}!", TryType.FullName); }
        }

        public int DisplayDepth
        {
            get { return InnerException is IDepthElement ? ((IDepthElement)InnerException).DisplayDepth + 1 : 1; }
        }
    }
}
