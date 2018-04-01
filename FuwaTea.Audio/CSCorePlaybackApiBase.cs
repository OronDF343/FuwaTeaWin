using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CSCore;
using CSCore.SoundOut;
using FuwaTea.Config;
using JetBrains.Annotations;

namespace FuwaTea.Audio
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Library name is CSCore")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "By design")]
    public abstract class CSCorePlaybackApiBase<TOut, TConfig> : IAudioPlaybackApi where TOut : ISoundOut where TConfig : CSCorePlaybackApiConfigBase
    {
        [SuppressMessage("ReSharper", "UnassignedField.Global", Justification = "Will be set by implementing class")]
        protected TOut SoundOut;
        protected readonly TConfig Config;
        protected IWaveSource WavSrc;

        public CSCorePlaybackApiBase(TConfig config)
        {
            Config = config;
            Config.PropertyChanged += ConfigOnPropertyChanged;
            CreateSoundOutWrapper();
        }

        protected abstract void CreateSoundOut();

        protected virtual void CreateSoundOutWrapper()
        {
            CreateSoundOut();
            SoundOut.Volume = Config.MasterVolume;
            SoundOut.Stopped += SoundOutOnStopped;
        }

        protected virtual void SoundOutOnStopped(object sender, PlaybackStoppedEventArgs args)
        {
            if (args.HasError) OnPlaybackError(new PlaybackErrorEventArgs(args.Exception));
            else OnPlaybackFinished();
        }

        protected virtual void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CSCorePlaybackApiConfigBase.MasterVolume))
                SoundOut.Volume = Config.MasterVolume;
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

    public abstract class CSCorePlaybackApiConfigBase : IConfigPage
    {
        public float MasterVolume { get; set; } = 1.0f;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
