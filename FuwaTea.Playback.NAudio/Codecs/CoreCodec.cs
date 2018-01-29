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

using System.Collections.Generic;
using DryIocAttributes;
using NAudio.Wave;

namespace FuwaTea.Playback.NAudio.Codecs
{
    //[NAudioExtension("NAudio built-in readers (non-MF)")]
    [Reuse(ReuseType.Singleton)]
    public class CoreCodec : ICodecProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { "wav|WAV Audio File", "aiff|AIFF Audio File" };
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }
}
