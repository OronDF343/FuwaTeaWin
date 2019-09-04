using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage.Lib
{
    public static class EnumUtils
    {
        public static T? ParseOrDefault<T>(this string str, bool ignoreCase = false) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(str)) return default;
            var b = Enum.TryParse(str, ignoreCase, out T v);
            return b ? v : (T?)null;
        }

        public static T? ConvertToEnum<T>(this uint val) where T : struct, Enum
        {
            return (T)Enum.ToObject(typeof(T), val);
        }

        public static bool HasFlags<T>(this T val, params T[] flags) where T : struct, Enum, IConvertible
        {
            var mask = flags.Aggregate(0UL, (s, e) => s | e.ToUInt64(null));
            return (val.ToUInt64(null) & mask) > 0;
        }

        public static IEnumerable<T> GetFlags<T>(this T val) where T : struct, Enum, IConvertible
        {
            return Enum.GetValues(typeof(T)).OfType<T>().Where(t => (val.ToUInt64(null) & t.ToUInt64(null)) > 0);
        }
    }
}
