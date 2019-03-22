using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Helpers
{
    public static class Utils
    {
        public static IEnumerable<T> TryCast<T>(this IEnumerable<object> o) where T : class
        {
            return o.Select(v =>
            {
                if (v is T d) return (T)d;
                try { return (T)Convert.ChangeType(v, typeof(T)); }
                catch { return null; }
            }).Where(v => v != null);
        }
        public static IEnumerable<T> TryCastToValueType<T>(this IEnumerable<object> o) where T : struct
        {
            return o.Select(v =>
            {
                if (v is T d) return (T?)d;
                try { return (T?)Convert.ChangeType(v, typeof(T)); }
                catch { return null; }
            }).Where(v => v != null).Cast<T>();
        }
    }
}
