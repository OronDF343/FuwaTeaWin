using System;
using System.Linq;

namespace Sage.Lib
{
    public static class StringUtils
    {
        public static byte[] UnHexify(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
        }

        public static string GetExtension(this string path)
        {
            return path.Substring(path.LastIndexOf('.') + 1).ToLowerInvariant();
        }
    }
}
