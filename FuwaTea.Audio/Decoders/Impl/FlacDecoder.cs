using System;
using System.Collections.Generic;
using CSCore;
using CSCore.Codecs.FLAC;
using FuwaTea.Audio.Files;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class FlacDecoder : ITrackDecoder
    {
        public IEnumerable<string> SupportedFormats => new[] { "flac" };
        public bool CanHandle(IFileHandle file)
        {
            bool r;
            using (var s = file.Stream)
            {
                r = s.VerifyMagic("fLaC");
            }
            return r;
        }

        public ISampleSource Handle(IFileHandle file)
        {
            return new FlacFile(file.Stream, FlacPreScanMode.Sync).ToSampleSource();
        }

        public bool UpdateMetadata(IFileHandle file)
        {
            // Use FlacPreScanMode.None?
            throw new NotImplementedException();
        }
    }
}
