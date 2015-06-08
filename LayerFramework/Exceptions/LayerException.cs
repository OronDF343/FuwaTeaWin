using System;
using FuwaTea.Common.Exceptions;

namespace LayerFramework.Exceptions
{
    public abstract class LayerException : Exception, IDepthElement
    {
        public LayerException()
        {
            DisplayDepth = 0;
        }

        public LayerException(string message)
            : base(message)
        {
            DisplayDepth = 0;
        }

        public LayerException(Exception innerException, int fallbackDisplayDepth = 1)
            : this("", innerException, fallbackDisplayDepth) { }

        public LayerException(string message, Exception innerException, int fallbackDisplayDepth = 1)
            : base(message, innerException)
        {
            DisplayDepth = innerException is IDepthElement ? ((IDepthElement)innerException).DisplayDepth + 1 : fallbackDisplayDepth;
        }

        // TODO: When C# 6.0 is released, add initializer = 0 and remove the assignment from the first two constructors.
        /// <summary>
        /// How many layers of Inner Exceptions contain useful info?
        /// </summary>
        public int DisplayDepth { get; private set; }

        public abstract string LayerName { get; }
    }
}
