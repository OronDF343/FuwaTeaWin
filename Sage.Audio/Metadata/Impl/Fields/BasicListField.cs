using System.Collections.Generic;
using Sage.Lib.Collections;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicListField : IListField
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

        void IMetadataField.SetFrom(string s)
        {
            if (Value == null) Value = new List<string> { s };
            else Value.Add(s);
        }

        void IMetadataField.SetFrom(uint s)
        {
            ((IMetadataField)this).SetFrom(s.ToString());
        }

        void IMetadataField.SetFrom(IEnumerable<string> s)
        {
            Value.AddRange(s);
        }
    }
}
