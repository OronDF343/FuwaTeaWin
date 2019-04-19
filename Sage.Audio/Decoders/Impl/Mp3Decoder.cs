using System;
using System.Collections.Generic;
using System.IO;
using CSCore;
using CSCore.Codecs.MP3;
using Sage.Audio.Files;
using Sage.Audio.Metadata.Impl.TagLib;
using Sage.Lib.Collections;
using TagLib;
using File = TagLib.File;

namespace Sage.Audio.Decoders.Impl
{
    public class Mp3Decoder : ITrackDecoder
    {
        public void UpdateMetadata(IFileHandle file)
        {
            var fha = new FileHandleAbstraction(file);
            var f = File.Create(fha, ReadStyle.Average);
            f.Mode = File.AccessMode.Read;
            var tag = TagLibUtils.ReadFrom(f);
            file.Metadata.AddOrSet(MetadataSource.Decoder, tag);
        }

        public IEnumerable<string> SupportedFormats => new[] { "mp3" };
        public bool CanHandle(IFileHandle ti)
        {
            bool r;
            using (var s = ti.OpenStream(FileAccess.Read))
            {
                r = s.VerifyMagic("ID3");
                s.Seek(0, SeekOrigin.Begin);
                r |= s.VerifyBitMask(new byte[] { 0xFF, 0xE0 });
            }
            return r;
        }

        public ISampleSource Handle(IFileHandle ti)
        {
            IWaveSource ws;
            var s = ti.OpenStream(FileAccess.Read);
            try
            {
                ws = new DmoMp3Decoder(s);
            }
            catch (Exception)
            {
                if (Mp3MediafoundationDecoder.IsSupported)
                    ws = new Mp3MediafoundationDecoder(s);
                else
                {
                    s.Close();
                    throw;
                }
            }
            return ws.ToSampleSource();
        }
    }
}
