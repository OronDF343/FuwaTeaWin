using System.Collections.Generic;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicNumericField : MetadataField, INumericField 
    {
        public BasicNumericField(uint maxValue = 0)
        {
            MaxValue = maxValue;
        }
        public uint MaxValue { get; }
        public uint? Value { get; set; }
        public override void SetFrom(string s)
        {
            Value = string.IsNullOrWhiteSpace(s) ? (uint?)null : uint.Parse(s);
        }

        public override void SetFrom(uint s)
        {
            Value = s;
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            SetFrom(s.FirstOrDefault());
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
