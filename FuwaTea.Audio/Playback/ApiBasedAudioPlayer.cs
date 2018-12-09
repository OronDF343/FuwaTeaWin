using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CSCore;
using DryIocAttributes;
using JetBrains.Annotations;

namespace FuwaTea.Audio.Playback
{
    [Reuse(ReuseType.Transient)]
    public class ApiBasedAudioPlayer : IAudioPlayer
    {
        protected readonly ApiSelector ApiSelector;
        private IAudioApi _api;
        
        public ApiBasedAudioPlayer([Import] ApiSelector apiSelector)
        {
            ApiSelector = apiSelector;
            ApiSelector.PropertyChanged += ApiSelectorOnPropertyChanged;
            Api = ApiSelector.SelectedImplementation;
        }

        protected virtual void ApiSelectorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(ApiSelector.SelectedImplementation))) return;
            Api = ApiSelector.SelectedImplementation;
        }

        [NotNull]
        protected IAudioApi Api
        {
            get => _api;
            private set => ChangeApi(value);
        }

        private void ChangeApi(IAudioApi value)
        {
            if (ReferenceEquals(_api, value)) return;
            if (_api != null)
            {
                Reset();
                _api.Reinitialized -= ApiOnReinitialized;
                _api.PlaybackFinished -= ApiOnPlaybackFinished;
                _api.PlaybackError -= ApiOnPlaybackError;
                _api.Dispose();
            }
            _api = value;
            if (_api != null)
            {
                _api.Reinitialized += ApiOnReinitialized;
                _api.PlaybackFinished += ApiOnPlaybackFinished;
                _api.PlaybackError += ApiOnPlaybackError;
            }
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

        public void Reset()
        {
            Unload();
        }

        public virtual void Load(IWaveSource dec)
        {
            try 
            {
                // TODO IMPORTANT: SampleSource + Effects
                // TODO IMPORTANT: Create interface wrapper so we don't have to depend on CSCore
                Api.Load(dec);
                StateTransition(AudioPlayerState.Loaded, TransitionInitiator.Manual);
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
                StateTransition(State, TransitionInitiator.Error, e);
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
                try
                {
                    Api.Pause();
                    StateTransition(AudioPlayerState.Paused, TransitionInitiator.Manual);
                }
                catch (Exception e)
                {
                    StateTransition(AudioPlayerState.Playing, TransitionInitiator.Error, e);
                }
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
