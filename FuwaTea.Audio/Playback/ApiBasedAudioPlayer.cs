using System;
using CSCore;
using DryIocAttributes;

namespace FuwaTea.Audio.Playback
{
    [Reuse(ReuseType.Transient)]
    public class ApiBasedAudioPlayer : IAudioPlayer
    {
        protected readonly IAudioApi Api;

        public ApiBasedAudioPlayer(IAudioApi api)
        {
            Api = api;
            Api.Reinitialized += ApiOnReinitialized;
            Api.PlaybackError += ApiOnPlaybackError;
            Api.PlaybackFinished += ApiOnPlaybackFinished;
        }

        protected virtual void ApiOnReinitialized(object sender, EventArgs eventArgs)
        {
            StateTransition(AudioPlayerState.NotReady, TransitionInitiator.Automatic);
        }

        protected virtual void ApiOnPlaybackFinished(object sender, EventArgs eventArgs)
        {
            StateTransition(AudioPlayerState.Stopped, TransitionInitiator.Automatic);
        }

        protected virtual void ApiOnPlaybackError(object sender, PlaybackErrorEventArgs args)
        {
            StateTransition(AudioPlayerState.Stopped, TransitionInitiator.Error, args.ErrorInfo);
        }

        public virtual void Load(IWaveSource dec)
        {
            try 
            {
                // TODO IMPORTANT: SampleSource vs WaveSource -- Effects
                // TODO IMPORTANT: Create interface wrapper so we don't have to depend on CSCore
                Api.Load(dec);
                StateTransition(AudioPlayerState.Stopped, TransitionInitiator.Manual);
            }
            catch (Exception e)
            {
                StateTransition(AudioPlayerState.NotReady, TransitionInitiator.Error, e);
            }
        }

        public virtual void Unload()
        {
            Api.Unload();
            StateTransition(AudioPlayerState.NotReady, TransitionInitiator.Manual);
        }

        public virtual void Play()
        {
            try 
            {
                Api.Play();
                StateTransition(AudioPlayerState.Playing, TransitionInitiator.Manual);
            }
            catch (Exception e)
            {
                StateTransition(AudioPlayerState.Stopped, TransitionInitiator.Error, e);
            }
        }

        public virtual void Stop()
        {
            Api.Stop();
            StateTransition(AudioPlayerState.Stopped, TransitionInitiator.Manual);
        }

        public virtual void Pause()
        {
            if (CanResume)
            {
                Api.Pause();
                StateTransition(AudioPlayerState.Paused, TransitionInitiator.Manual);
            }
            else
            {
                Stop();
            }
        }

        public virtual TimeSpan Duration => Api.Duration;
        public virtual TimeSpan Position { get => Api.Position; set => Api.Position = value; }
        public virtual bool CanResume => Api.CanResume;
        public virtual bool CanSeek => Api.CanSeek;
        public virtual AudioPlayerState State { get; private set; }

        public event AudioPlayerStateChangedEventHandler StateChanged;

        protected virtual void StateTransition(AudioPlayerState newState, TransitionInitiator ti, object errorInfo = null)
        {
            var oldState = State;
            State = newState;
            StateChanged?.Invoke(this, new AudioPlayerStateChangedEventArgs(oldState, newState, ti, errorInfo));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Api.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
