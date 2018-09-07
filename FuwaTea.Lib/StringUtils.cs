using System;
using System.Collections.Generic;
using System.Linq;

namespace FuwaTea.Lib
{
    public static class StringUtils
    {
        public static IEnumerable<string> GetExtensions(IEnumerable<string> exts)
        {
            return from e in exts
                   let x = e.IndexOf('|')
                   select "." + e.Substring(0, x > -1 ? x : e.Length);
        }

        public static IEnumerable<string> GetExtensions(this IFileHandler fh)
        {
            return GetExtensions(fh.SupportedFileTypes);
        }

        public static Dictionary<string, string> GetExtensionsInfo(IEnumerable<string> exts)
        {
            return (from w in exts
                    let x = w.IndexOf('|')
                    select new KeyValuePair<string, string>(w.Substring(0, x > -1 ? x : w.Length),
                                                            w.Substring(x > -1 ? x + 1 : w.Length - 1))).ToDictionary(p => p.Key, p => p.Value);
        }

        public static Dictionary<string, string> GetExtensionsInfo(this IFileHandler fh)
        {
            return GetExtensionsInfo(fh.SupportedFileTypes);
        }

        public static byte[] UnHexify(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
        }

        public static string GetExtension(this string path)
        {
            return path.Substring(path.LastIndexOf('.') + 1);
        }
    }
}
