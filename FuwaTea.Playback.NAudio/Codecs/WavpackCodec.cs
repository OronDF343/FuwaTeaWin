using System.Collections.Generic;
using System.ComponentModel.Composition;
using NAudio.Wave;

namespace FuwaTea.Playback.NAudio.Codecs
{
    //[NAudioExtension("Wavpack file reader")]
    [Export(typeof(ICodecProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WavpackCodec : ICodecProvider
    {
        public WaveStream CreateWaveStream(string path)
        {
            return new WavpackReader(path);
        }

        public IEnumerable<string> SupportedFileTypes => new[] { "wv|Wavpack Audio File" };
        public bool IsSampleProvider => false;
        public bool CanResume => true;
        public bool CanSeek => true;
    }
}
