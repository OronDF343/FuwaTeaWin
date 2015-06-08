using System.Collections.Generic;
using NAudio.Flac;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    [DataElement("FLAC file reader (non-MF)")]
    public class FlacCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new FlacReader(path);
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] {".flac"}; } }
        public bool IsSampleProvider { get { return true; } }
        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return true; } }
    }
}
