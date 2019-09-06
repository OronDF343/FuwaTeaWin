using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Lib;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicEnumField<T> : MetadataField, IEnumField<T> where T : struct, Enum, IConvertible
    {
        public T? Value { get; set; }
        public override void SetFrom(string s)
        {
            StringValue = s;
        }

        public override void SetFrom(uint s)
        {
            Value = s.ConvertToEnum<T>();
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            SetFrom(s.FirstOrDefault());
        }

        public virtual string StringValue { get => ToString(); set => value.ParseOrDefault<T>(true); }
        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
