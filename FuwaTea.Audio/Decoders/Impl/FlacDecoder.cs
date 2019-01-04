using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata.Impl.TagLib;
using FuwaTea.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace FuwaTea.Audio.Decoders.Impl
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

        public ISampleSource Handle(IFileHandle file)
        {
            return new FlacFile(file.OpenStream(FileAccess.Read), FlacPreScanMode.Sync).ToSampleSource();
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
