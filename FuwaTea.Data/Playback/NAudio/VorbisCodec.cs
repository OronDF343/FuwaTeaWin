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
