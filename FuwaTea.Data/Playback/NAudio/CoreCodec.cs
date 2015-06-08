using System.Collections.Generic;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("NAudio built-in readers (non-MF)")]
    public class CoreCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new AudioFileReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] {".mp3", ".wav", ".aiff"}; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }
}
