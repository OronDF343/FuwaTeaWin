using System;

namespace FuwaTea.Audio.Playback 
{
    public class AudioPlayerStateChangedEventArgs : EventArgs
    {
        public AudioPlayerStateChangedEventArgs(AudioPlayerState oldState, AudioPlayerState newState, TransitionInitiator transitionInitiator, object errorInfo = null)
        {
            OldState = oldState;
            NewState = newState;
            TransitionInitiator = transitionInitiator;
            ErrorInfo = errorInfo;
        }
        public AudioPlayerState OldState { get; }
        public AudioPlayerState NewState { get; }
        public TransitionInitiator TransitionInitiator { get; }
        public object ErrorInfo { get; }
    }
}