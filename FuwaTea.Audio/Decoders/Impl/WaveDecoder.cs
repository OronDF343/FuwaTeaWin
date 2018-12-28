using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.MediaFoundation;
using DryIocAttributes;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata.Impl.TagLib;
using FuwaTea.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class WaveDecoder : ITrackDecoder
    {
        public void UpdateMetadata(IFileHandle file)
        {
            var fha = new FileHandleAbstraction(file);
            var f = File.Create(fha, ReadStyle.Average);
            f.Mode = File.AccessMode.Read;
            var tag = TagLibUtils.ReadFrom(f);
            file.Metadata.AddOrSet(MetadataSource.Decoder, tag);
        }

        public IEnumerable<string> SupportedFormats => new[] { "wav", "wave" };
        public bool CanHandle(IFileHandle ti)
        {
            bool r;
            using (var s = ti.OpenStream(FileAccess.Read))
            {
                r = s.VerifyMagic("RIFF");
                s.Seek(4, SeekOrigin.Current);
                r &= s.VerifyMagic("WAVEfmt ");
            }
            return r;
        }

        public ISampleSource Handle(IFileHandle ti)
        {
            var s = ti.OpenStream(FileAccess.Read);
            IWaveSource res = new WaveFileReader(s);
            if (res.WaveFormat.WaveFormatTag != AudioEncoding.Pcm &&
                res.WaveFormat.WaveFormatTag != AudioEncoding.IeeeFloat &&
                res.WaveFormat.WaveFormatTag != AudioEncoding.Extensible)
            {
                res.Dispose();
                res = new MediaFoundationDecoder(s);
            }
            return res.ToSampleSource();
        }
    }
}
