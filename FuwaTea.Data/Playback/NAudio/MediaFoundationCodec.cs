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
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("MediaFoundation on Windows Vista and higher (MP3, WMA, M4A, AAC) (M4A/AAC on Windows Vista requires KB2117917)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.0.0.0")]
    public class MediaFoundationVistaCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { ".mp3", ".wma", ".m4a", ".aac" };
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }

    [DataElement("MediaFoundation on Windows 7 and higher (ADTS)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.1.0.0")]
    public class MediaFoundationWin7Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { ".adts" };
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }

    [DataElement("MediaFoundation on Windows 8 and higher (AC3)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.2.0.0")]
    public class MediaFoundationWin8Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { ".ac3" };
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }

    [DataElement("MediaFoundation on Windows 10 and higher (FLAC)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "10.0.0.0")]
    public class MediaFoundationWin10Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { ".flac" };
        public bool IsSampleProvider => true;
        public bool CanResume => true;
        public bool CanSeek => true;
    }
}
