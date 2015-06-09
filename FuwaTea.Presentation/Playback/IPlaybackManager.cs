using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FuwaTea.Common.Models;
using FuwaTea.Logic.Playlist;

namespace FuwaTea.Presentation.Playback
{
    public interface IPlaybackManager : IPlaylistPositionManager, IPresentationElement, IDisposable
    {
        bool IsSomethingLoaded { get; }
        IEnumerable<string> SupportedFileTypes { get; }

        void PlayPauseResume();
        void Stop();
        PlaybackState CurrentState { get; }

        LoopTypes LoopType { get; set; }

        /// <summary>
        /// Gets the duration of the currently loaded file.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets or sets the current position in the loaded file.
        /// </summary>
        TimeSpan Position { get; set; }
        /// <summary>
        /// Raise PropertyChanged for Position.
        /// </summary>
        void SendPositionUpdate();

        /// <summary>
        /// 
        /// </summary>
        bool CanResume { get; }
        /// <summary>
        /// 
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// Gets or sets the playback volume. Default is 1.0.
        /// </summary>
        decimal Volume { get; set; }
        /// <summary>
        /// Gets or sets the left channel volume. Default/max is 1.0.
        /// </summary>
        decimal LeftVolume { get; set; }
        /// <summary>
        /// Gets or sets the right channel volume. Default/max is 1.0.
        /// </summary>
        decimal RightVolume { get; set; }

        /// <summary>
        /// An event which must be fired only if there is an error during playback (anywhere except for the Load function).
        /// </summary>
        event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        /// <summary>
        /// 
        /// </summary>
        bool IsEqualizerSupported { get; }
        /// <summary>
        /// Gets or sets whether to enable EQ
        /// </summary>
        bool EnableEqualizer { get; set; }
        /// <summary>
        /// Gets an observable collection of the bands used in the EQ
        /// </summary>
        ObservableCollection<EqualizerBand> EqualizerBands { get; }
    }

    public enum LoopTypes { None, Single, All }

    public enum PlaybackState { Stopped = 0, Paused = 1, Playing = 2 }
}
