using System;
using System.Runtime.Serialization;

namespace FTWPlayer.Skins
{
    [Serializable]
    public class SkinLoadException : Exception
    {
        /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null. </exception>
        protected SkinLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public SkinLoadException() { }

        public SkinLoadException(string message)
            : base(message) { }

        public SkinLoadException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
