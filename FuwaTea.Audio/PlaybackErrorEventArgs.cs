using System;

namespace FuwaTea.Audio
{
    public class PlaybackErrorEventArgs : EventArgs
    {
        public PlaybackErrorEventArgs(object errorInfo)
        {
            ErrorInfo = errorInfo;
        }

        public object ErrorInfo { get; }
    }
}