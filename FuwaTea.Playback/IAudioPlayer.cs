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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using FuwaTea.Lib;

namespace FuwaTea.Playback
{
    [InheritedExport]
    public interface IAudioPlayer : IPlaybackElement, IFileHandler, IDisposable
    {
        /// <summary>
        /// Prepare to play an audio file.
        /// </summary>
        /// <param name="path">The path to the audio file</param>
        void Load(string path);
        /// <summary>
        /// Unloads the currently loaded file. This includes stopping playback if necessary.
        /// </summary>
        void Unload();
        /// <summary>
        /// Gets a boolean value indicating whether a file is currently loaded.
        /// </summary>
        bool IsSomethingLoaded { get; }

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
        /// Resume playback of the current file. Will be called only if currently paused.
        /// </summary>
        void Resume();

        /// <summary>
        /// Gets the duration of the currently loaded file.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets or sets the current position in the loaded file.
        /// </summary>
        TimeSpan Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool CanResume { get; }
        /// <summary>
        /// 
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// An event which must be fired only when the end of the audio file is reached (and playback stops).
        /// </summary>
        event EventHandler PlaybackFinished;

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
        /// Gets an observable collection of the bands used in the EQ.
        /// Will only be set up to once during runtime! 
        /// Leave this at null, and it will be set before Load is called!
        /// </summary>
        ObservableCollection<EqualizerBand> EqualizerBands { get; set; }
    }
}
