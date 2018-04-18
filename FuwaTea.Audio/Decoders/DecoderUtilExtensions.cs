using System.IO;
using System.Linq;

namespace FuwaTea.Audio.Decoders
{
    public static class DecoderUtilExtensions
    {
        public static bool VerifyMagic(this Stream file, byte[] magic)
        {
            var darr = new byte[magic.Length];
            file.Read(darr, 0, darr.Length);
            return magic.SequenceEqual(darr);
        }
    }
}
