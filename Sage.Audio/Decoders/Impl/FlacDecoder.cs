using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;
using Sage.Audio.Files;
using Sage.Audio.Metadata.Impl.TagLib;
using Sage.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace Sage.Audio.Decoders.Impl
{
    public class FlacDecoder : ITrackDecoder
    {
        public IEnumerable<string> SupportedFormats => new[] { "flac" };
        public bool CanHandle(IFileHandle file)
        {
            bool r;
            using (var s = file.OpenStream(FileAccess.Read))
                r = s.VerifyMagic("fLaC");
            return r;
        }

        public IWaveSource Handle(IFileHandle file)
        {
            return new FlacFile(file.OpenStream(FileAccess.Read), FlacPreScanMode.Sync);
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
