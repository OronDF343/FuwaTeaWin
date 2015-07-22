#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FuwaTea.Common.Models;

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
