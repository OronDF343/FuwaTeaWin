using System.Collections.Generic;
using System.IO;
using CSCore;
using Sage.Audio.Files;
using Sage.Audio.Metadata.Impl.TagLib;
using Sage.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace Sage.Audio.Decoders.Wavpack
{
    public class WavpackDecoder : ITrackDecoder
    {
        public IEnumerable<string> SupportedFormats => new[] { "wv" };
        public bool CanHandle(IFileHandle ti)
        {
            bool r;
            using (var s = ti.OpenStream(FileAccess.Read)) 
                r = s.VerifyMagic("wvpk");
            return r;
        }

        public IWaveSource Handle(IFileHandle ti)
        {
            return new WavpackWaveSource(ti.OpenStream(FileAccess.Read));
        }

        public void UpdateMetadata(IFileHandle file)
        {
            var fha = new FileHandleAbstraction(file);
            var f = File.Create(fha, ReadStyle.Average);
            f.Mode = File.AccessMode.Read;
            var tag = TagLibUtils.ReadFrom(f);
            file.Metadata.AddOrSet(MetadataSource.Decoder, tag);
        }
    }
}
