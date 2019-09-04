using System.Collections.Generic;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicTextField : ITextField
    {
        public BasicTextField(uint maxLength = 0)
        {
            MaxLength = maxLength;
        }
        public virtual uint MaxLength { get; }
        public string Value { get; set; }
        public virtual void SetFrom(string s)
        {
            Value = s;
        }

        void IMetadataField.SetFrom(uint s)
        {
            SetFrom(s.ToString());
        }

        void IMetadataField.SetFrom(IEnumerable<string> s)
        {
            SetFrom(s.FirstOrDefault());
        }
    }
}
