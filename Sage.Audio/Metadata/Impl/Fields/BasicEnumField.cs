using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Lib;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicEnumField<T> : IEnumField<T> where T : struct, Enum, IConvertible
    {
        public T? Value { get; set; }
        public virtual void SetFrom(string s)
        {
            StringValue = s;
        }

        public void SetFrom(uint s)
        {
            Value = s.ConvertToEnum<T>();
        }

        void IMetadataField.SetFrom(IEnumerable<string> s)
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
