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
using LayerFramework.Attributes;
using NAudio.Flac;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("FLAC file reader (non-MF)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.LessThan, "10.0.0.0")]
    public class FlacCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new FlacReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] {".flac"};
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }
}
