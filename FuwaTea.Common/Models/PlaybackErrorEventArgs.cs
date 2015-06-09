using System;

namespace FuwaTea.Common.Models
{
    public class PlaybackErrorEventArgs : EventArgs
    {
        public PlaybackErrorEventArgs(string message)
        {
            Message = message;
        }

        public PlaybackErrorEventArgs(string message, Exception innerException)
            : this(message)
        {
            InnerException = innerException;
        }

        public PlaybackErrorEventArgs(string message, bool willContinue)
            : this(message)
        {
            WillPlaybackContinue = willContinue;
        }

        public PlaybackErrorEventArgs(string message, Exception innerException, bool willContinue)
            : this(message, innerException)
        {
            WillPlaybackContinue = willContinue;
        }

        public string Message { get; set; }

        public bool WillPlaybackContinue { get; set; }

        public Exception InnerException { get; set; }
    }
}
