using System;
using CSCore;
using CSCore.Codecs.FLAC;
using FuwaTea.Audio.Files;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class FlacDecoder : ITrackDecoder
    {
        public string[] Extensions => new[] { "flac" };
        public bool CanDecode(IFileHandle file)
        {
            bool r;
            using (var s = file.Stream)
            {
                r = s.VerifyMagic("fLaC");
            }
            return r;
        }

        public ISampleSource Begin(IFileHandle file)
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
