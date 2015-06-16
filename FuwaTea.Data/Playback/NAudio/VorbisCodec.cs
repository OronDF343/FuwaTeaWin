﻿#region License
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

using System.Collections.Generic;
using NAudio.Vorbis;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("Vorbis file reader")]
    public class VorbisCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new VorbisWaveReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] {".ogg"}; } }
        public bool IsSampleProvider { get { return false; } } // TODO: temporary workaround, change when NVorbis is updated
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }
}
