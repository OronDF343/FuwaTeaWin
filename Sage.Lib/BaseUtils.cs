using System;

namespace Sage.Lib
{
    public static class BaseUtils
    {
        public static TOut? NullableCast<TIn, TOut>(this TIn? src) where TIn : struct, IConvertible where TOut : struct
        {
            return src != null ? (TOut?)Convert.ChangeType(src.Value, typeof(TOut)) : null;
        }
    }
}
