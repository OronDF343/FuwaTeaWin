using System;

namespace FTWPlayer.Skins
{
    public class SkinLoadException : Exception
    {
        public SkinLoadException() { }

        public SkinLoadException(string message)
            : base(message) { }

        public SkinLoadException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
