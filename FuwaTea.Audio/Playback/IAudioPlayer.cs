using System;
using System.ComponentModel.Composition;
using CSCore;

namespace FuwaTea.Audio.Playback
{
    [InheritedExport]
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// Prepare to play audio.
        /// </summary>
        /// <param name="dec">The decoded data provider</param>
        void Load(IWaveSource dec);
        /// <summary>
        /// Unloads the currently loaded file. This includes stopping playback if necessary.
        /// </summary>
        void Unload();

        /// <summary>
        /// Begin playback of the loaded file from the beginning of the file.
        /// </summary>
        void Play();
        /// <summary>
        /// Stops playback of the current file.
        /// </summary>
        void Stop();
        /// <summary>
        /// Pause playback of the current file. Will be called only if currently playing.
        /// </summary>
        void Pause();

        /// <summary>
        /// Gets the duration of the currently loaded file.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets or sets the current position in the loaded file.
        /// </summary>
        TimeSpan Position { get; set; }

        /// <summary>
        /// Gets whether resuming is supported
        /// </summary>
        bool CanResume { get; }
        /// <summary>
        /// Gets whether seeking is supported
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// Gets the current playback state
        /// </summary>
        AudioPlayerState State { get; }

        /// <summary>
        /// Fires when the playback state changes
        /// </summary>
        event AudioPlayerStateChangedEventHandler StateChanged;
    }
}