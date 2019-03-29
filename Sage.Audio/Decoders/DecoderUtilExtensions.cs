using System.IO;
using System.Linq;
using System.Text;

namespace Sage.Audio.Decoders
{
    public static class DecoderUtilExtensions
    {
        public static bool VerifyBitMask(this Stream file, byte[] magic)
        {
            var darr = new byte[magic.Length];
            file.Read(darr, 0, darr.Length);
            return !magic.Where((t, i) => (t & darr[i]) != t).Any();
        }
        public static bool VerifyMagic(this Stream file, byte[] magic)
        {
            var darr = new byte[magic.Length];
            file.Read(darr, 0, darr.Length);
            return magic.SequenceEqual(darr);
        }

        public static bool VerifyMagic(this Stream file, string asciiMagic)
        {
            var magic = Encoding.ASCII.GetBytes(asciiMagic);
            var darr = new byte[magic.Length];
            file.Read(darr, 0, darr.Length);
            return magic.SequenceEqual(darr);
        }
    }
}
