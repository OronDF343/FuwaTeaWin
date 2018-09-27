using System;
using System.Linq;

namespace FuwaTea.Lib
{
    public static class EnumUtils
    {
        public static T ParseOrDefault<T>(this string str, bool ignoreCase = false) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(str)) return default;
            var b = Enum.TryParse(str, ignoreCase, out T v);
            return b ? v : default;
        }
        public static bool HasFlags<T>(this T val, params T[] flags) where T : struct, Enum, IConvertible
        {
            var mask = flags.Aggregate(0UL, (s, e) => s | e.ToUInt64(null));
            return (val.ToUInt64(null) & mask) > 0;
        }
    }
}
