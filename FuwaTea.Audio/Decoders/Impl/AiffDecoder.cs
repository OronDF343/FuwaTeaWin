using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.AIFF;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata.Impl.TagLib;
using FuwaTea.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class AiffDecoder : ITrackDecoder
    {
        public void UpdateMetadata(IFileHandle file)
        {
            var fha = new FileHandleAbstraction(file);
            var f = File.Create(fha, ReadStyle.Average);
            f.Mode = File.AccessMode.Read;
            var tag = TagLibUtils.ReadFrom(f);
            file.Metadata.AddOrSet(MetadataSource.Decoder, tag);
        }

        public IEnumerable<string> SupportedFormats => new[] { "aiff", "aif", "aifc" };
        public bool CanHandle(IFileHandle ti)
        {
            bool r;
            using (var s = ti.OpenStream(FileAccess.Read))
            {
                r = s.VerifyMagic("FORM");
                r &= s.ReadByte() == 0;
            }
            return r;
        }

        public ISampleSource Handle(IFileHandle ti)
        {
            var s = ti.OpenStream(FileAccess.Read);
            var res = new AiffReader(s);
            return res.ToSampleSource();
        }
    }
}