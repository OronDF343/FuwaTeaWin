using System;
using System.ComponentModel.Composition;
using CSCore;

namespace Sage.Audio.Playback
{
    [InheritedExport]
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// Resets / re-initializes the playback engine.
        /// </summary>
        /// <remarks>Should transition: NotReady, Manual. Should not throw</remarks>
        void Reset();
        /// <summary>
        /// Load a file and prepare to play audio.
        /// </summary>
        /// <remarks>Should transition: Loaded, Manual. On error: NotReady</remarks>
        /// <param name="dec">The decoded data provider</param>
        void Load(IWaveSource dec);
        /// <summary>
        /// Unloads the currently loaded file.
        /// </summary>
        /// <remarks>Precondition: >= Loaded. Should transition: NotReady, Manual. Should not throw - call Reset() instead</remarks>
        void Unload();

        /// <summary>
        /// Begin or resume playback of the loaded file.
        /// </summary>
        /// <remarks>Precondition: >= Loaded, &lt; Playing. Should transition: Playing, Manual. On error: Transition with no change to state</remarks>
        void Play();
        /// <summary>
        /// Stops playback of the loaded file and resets the position to the beginning.
        /// </summary>
        /// <remarks>Precondition: >= Paused. Should transition: Stopped, Manual. On error: Avoid throwing. Please ensure that playback is always stopped; If Unload/Reset routines must be used: NotReady.</remarks>
        void Stop();
        /// <summary>
        /// Pause playback of the loaded file. Will be called only if currently playing.
        /// </summary>
        /// <remarks>Precondition: >= Playing. Should transition: Paused, Manual. On error: Playing</remarks>
        void Pause();

        /// <summary>
        /// Gets the duration of the currently loaded file.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets or sets the current position in the loaded file.
        /// </summary>
        /// <remarks>Setter precondition: >= Loaded</remarks>
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