using System;
using System.Collections.Generic;
using LayerFramework.Attributes;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("MediaFoundation on Windows Vista and higher (WMA, M4A, AAC) (M4A/AAC on Windows Vista requires KB2117917)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.0.0.0")]
    public class MediaFoundationVistaCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] {".wma", ".m4a", ".aac"}; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }

    [DataElement("MediaFoundation on Windows 7 and higher (ADTS)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.1.0.0")]
    public class MediaFoundationWin7Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] { ".adts" }; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }

    [DataElement("MediaFoundation on Windows 8 and higher (AC3)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.2.0.0")]
    public class MediaFoundationWin8Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] { ".ac3" }; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }

    [DataElement("MediaFoundation on Windows 10 and higher (FLAC)")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "10.0.0.0")]
    public class MediaFoundationWin10Codec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] { ".flac" }; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }
}
