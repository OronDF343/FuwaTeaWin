using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;

namespace FuwaTea.Audio.Decoders.CSCore
{
    [AudioDecoder("FLAC")]
    public class FlacDecoder : SampleAggregatorBase, IAudioDecoder
    {
        public FlacDecoder(Stream stream)
            : base(new FlacFile(stream).ToSampleSource()) { }
    }
}
