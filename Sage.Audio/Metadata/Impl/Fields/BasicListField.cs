using System.Collections.Generic;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicListField : IListField
    {
        public BasicListField() { }
        public BasicListField(uint maxCount = 0, uint maxLength = 0)
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

        void IMetadataField.ParseFrom(string s)
        {
            if (Value == null) Value = new List<string> { s };
            else Value.Add(s);
        }
    }
}
