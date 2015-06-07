using System;
using FuwaTea.Common.Exceptions;

namespace LayerFramework.Exceptions
{
    public class LayerException : Exception, IDepthElement
    {
        public LayerException(string layer)
        {
            LayerName = layer;
            DisplayDepth = 0;
        }

        public LayerException(string layer, string message)
            : base(message)
        {
            LayerName = layer;
            DisplayDepth = 0;
        }

        public LayerException(string layer, Exception innerException, int fallbackDisplayDepth = 1)
            : this(layer, "", innerException, fallbackDisplayDepth) { }

        public LayerException(string layer, string message, Exception innerException, int fallbackDisplayDepth = 1)
            : base(message, innerException)
        {
            LayerName = layer;
            DisplayDepth = innerException is IDepthElement ? ((IDepthElement)innerException).DisplayDepth + 1 : fallbackDisplayDepth;
        }

        // TODO: When C# 6.0 is released, add initializer = 0 and remove the assignment from the first two constructors.
        /// <summary>
        /// How many layers of Inner Exceptions contain useful info?
        /// </summary>
        public int DisplayDepth { get; private set; }

        public string LayerName { get; set; }
    }
}
