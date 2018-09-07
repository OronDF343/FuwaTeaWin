using System;
using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;
using DryIocAttributes;
using FuwaTea.Audio.Files;

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
            // Use FlacPreScanMode.None?
            throw new NotImplementedException();
        }
    }
}
