using System.Collections.Generic;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicTextField : MetadataField, ITextField
    {
        public BasicTextField(uint maxLength = 0)
        {
            MaxLength = maxLength;
        }
        public virtual uint MaxLength { get; }
        public string Value { get; set; }
        public override void SetFrom(string s)
        {
            Value = s;
        }

        public override void SetFrom(uint s)
        {
            SetFrom(s.ToString());
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            SetFrom(s.FirstOrDefault());
        }

        public override string ToString()
        {
            return Value ?? "";
        }
    }
}
