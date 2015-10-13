using System.Collections.Generic;
using NAudio.Wave;

namespace FuwaTea.Playback.NAudio.Codecs
{
    [Codec("Wavpack file reader")]
    public class WavpackCodec : IWaveStreamProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new WavpackReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { ".wv" };
        public bool IsSampleProvider => false;
        public bool CanResume => true;
        public bool CanSeek => true;
    }
}
