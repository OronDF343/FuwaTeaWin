using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CSCore;
using CSCore.SoundOut;

namespace Sage.Audio.Playback.CSCore
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Library name is CSCore")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "By design")]
    public abstract class CSCoreApiBase<TOut, TConfig> : IAudioApi
        where TOut : ISoundOut
        where TConfig : CSCoreApiConfigBase
    {
        protected TOut SoundOut;
        protected readonly TConfig Config;
        protected IWaveSource WavSrc;

        public CSCoreApiBase(TConfig config)
        {
            Config = config;
            Config.PropertyChanged += ConfigOnPropertyChanged;
            CreateSoundOutWrapper();
        }

        protected abstract TOut CreateSoundOut();

        protected virtual void CreateSoundOutWrapper()
        {
            SoundOut = CreateSoundOut();
            SoundOut.Stopped += SoundOutOnStopped;
        }

        protected virtual void SoundOutOnStopped(object sender, PlaybackStoppedEventArgs args)
        {
            if (args.HasError) OnPlaybackError(new PlaybackErrorEventArgs(args.Exception));
            else OnPlaybackFinished();
        }
        
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause", Justification = "There is no way to know easily if the output is already initialized")]
        protected virtual void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CSCoreApiConfigBase.MasterVolume))
                try { SoundOut.Volume = Config.MasterVolume; }
                catch { }
            else
            {
                Unload();
                OnReinitialized();
            }
        }

        public void Load(IWaveSource dec)
        {
            // TODO IMPORTANT: Do we need to always unload first like with NAudio?
            WavSrc = dec;
            SoundOut.Initialize(WavSrc);
            SoundOut.Volume = Config.MasterVolume;
        }

        public virtual bool CanResume => true;
        public bool CanSeek => WavSrc.CanSeek;
        public TimeSpan Duration => WavSrc.GetLength();
        public TimeSpan Position { get => WavSrc.GetPosition(); set => WavSrc.SetPosition(value); }
        public virtual void Unload()
        {
            SoundOut?.Dispose();
            WavSrc?.Dispose();
            WavSrc = null;
            CreateSoundOutWrapper();
        }

        public virtual void Play()
        {
            SoundOut.Play();
        }

        public virtual void Stop()
        {
            SoundOut.Stop();
        }

        public virtual void Pause()
        {
            SoundOut.Pause();
        }

        public event EventHandler Reinitialized;
        public event EventHandler PlaybackFinished;
        public event PlaybackErrorEventHandler PlaybackError;
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SoundOut?.Dispose();
                WavSrc?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnReinitialized()
        {
            Reinitialized?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPlaybackFinished()
        {
            PlaybackFinished?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPlaybackError(PlaybackErrorEventArgs args)
        {
            PlaybackError?.Invoke(this, args);
        }
    }
}