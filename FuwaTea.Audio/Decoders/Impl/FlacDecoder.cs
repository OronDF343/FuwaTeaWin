using System;
using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;
using DryIocAttributes;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata.Impl.TagLib;
using TagLib;
using File = TagLib.File;

namespace FuwaTea.Audio.Decoders.Impl
{
    [Reuse(ReuseType.Singleton)]
    public class FlacDecoder : ITrackDecoder
    {
        public IEnumerable<string> SupportedFormats => new[] { "flac" };
        public bool CanHandle(IFileHandle file)
        {
            bool r;
            using (var s = file.OpenStream(FileAccess.Read))
            {
                r = s.VerifyMagic("fLaC");
            }
            return r;
        }

        public ISampleSource Handle(IFileHandle file)
        {
            return new FlacFile(file.OpenStream(FileAccess.Read), FlacPreScanMode.Sync).ToSampleSource();
        }

        public bool UpdateMetadata(IFileHandle file)
        {
            var fha = new FileHandleAbstraction(file);
            var f = File.Create(fha, ReadStyle.Average);
            f.Mode = File.AccessMode.Read;
            var m = f.GetTag(TagTypes.FlacMetadata) as TagLib.Flac.Metadata;
            //return file.Metadata.ReadFrom(m);
            throw new NotImplementedException();
        }
    }
}
