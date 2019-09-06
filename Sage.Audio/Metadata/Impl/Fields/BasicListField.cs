using System.Collections.Generic;
using System.Linq;
using Sage.Lib.Collections;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicListField : MetadataField, IListField
    {
        public BasicListField()
        {
            Value = new List<string>();
        }
        public BasicListField(uint maxCount = 0, uint maxLength = 0) : this()
        {
            MaxCount = maxCount;
            MaxLength = maxLength;
        }

        public virtual uint MaxCount { get; }

        /// <summary>
        /// For list fields, this refers to the max total size of the list (in bytes, when such a limit is applicable).
        /// </summary>
        /// <remarks>This value may be overridden to make it dependent on the number of entries.</remarks>
        public virtual uint MaxLength { get; }
        
        public IList<string> Value { get; set; }

        public override void SetFrom(string s)
        {
            if (Value == null) Value = new List<string> { s };
            else Value.Add(s);
        }

        public override void SetFrom(uint s)
        {
            ((IMetadataField)this).SetFrom(s.ToString());
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            Value.AddRange(s);
        }

        public override string ToString()
        {
            return Value?.FirstOrDefault() ?? "";
        }

        public override IEnumerable<string> ToStringEnumerable()
        {
            return Value ?? new string[] { };
        }
    }
}
