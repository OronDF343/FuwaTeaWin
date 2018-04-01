using System;
using System.ComponentModel.Composition;
using CSCore;

namespace FuwaTea.Audio
{
    [InheritedExport]
    public interface IAudioPlaybackApi : IDisposable
    {
        void Load(IWaveSource dec);
        bool CanResume { get; }
        bool CanSeek { get; }
        TimeSpan Duration { get; }
        TimeSpan Position { get; set; }
        void Unload();
        void Play();
        void Stop();
        void Pause();

        event EventHandler Reinitialized;
        event EventHandler PlaybackFinished;
        event PlaybackErrorEventHandler PlaybackError;
    }
}
