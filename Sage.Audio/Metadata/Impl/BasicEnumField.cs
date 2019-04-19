using System;
using Sage.Lib;

namespace Sage.Audio.Metadata.Impl
{
    public class BasicEnumField<T> : IEnumField<T> where T : struct, Enum, IConvertible
    {
        public T? Value { get; set; }
        public virtual string StringValue { get => ToString(); set => value.ParseOrDefault<T>(true); }
        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
