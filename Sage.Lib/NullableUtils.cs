using System;

namespace Sage.Lib
{
    public static class NullableUtils
    {
        public static TOut? NullableCast<TIn, TOut>(this TIn? src) where TIn : struct, IConvertible where TOut : struct
        {
            return src != null ? (TOut?)Convert.ChangeType(src.Value, typeof(TOut)) : null;
        }

        public static TOut? NullableParse<TOut>(this string src, Func<string, TOut> parseFunc) where TOut : struct
        {
            return src != null ? (TOut?)parseFunc.Invoke(src) : null;
        }

        public static TOut? NullableParse<TOut>(this string src, Func<string, string, TOut> parseFunc, string format) where TOut : struct
        {
            return src != null ? (TOut?)parseFunc.Invoke(src, format) : null;
        }
    }
}
