using System.Collections.Generic;
using System.Linq;
using Sage.Lib.Collections;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicListWithDescriptorsField : MetadataField, IListWithDescriptorsField
    {
        public BasicListWithDescriptorsField()
        {
            Value = new List<EntryWithDescriptors>();
        }
        public BasicListWithDescriptorsField(uint maxCount = 0, uint maxLength = 0, bool descriptionSupported = true, bool descriptionRequired = true, bool languageSupported = true, bool languageRequired = true)
            : this()
        {
            MaxCount = maxCount;
            MaxLength = maxLength;
            DescriptionSupported = descriptionSupported;
            DescriptionRequired = descriptionRequired;
            LanguageSupported = languageSupported;
            LanguageRequired = languageRequired;
        }

        public virtual uint MaxCount { get; }

        /// <summary>
        /// For list fields, this refers to the max total size of the list (in bytes, when such a limit is applicable).
        /// </summary>
        /// <remarks>This value may be overridden to make it dependent on the number of entries.</remarks>
        public virtual uint MaxLength { get; }

        public bool DescriptionSupported { get; }
        public bool DescriptionRequired { get; }
        public bool LanguageSupported { get; }
        public bool LanguageRequired { get; }

        public IList<EntryWithDescriptors> Value { get; set; }

        public override void SetFrom(string s)
        {
            // Not supported
            var e = new EntryWithDescriptors(s);
            if (Value == null) Value = new List<EntryWithDescriptors> { e };
            else Value.Add(e);
        }

        public override void SetFrom(uint s)
        {
            ((IMetadataField)this).SetFrom(s.ToString());
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            Value.AddRange(s.Select(i => new EntryWithDescriptors(i)));
        }

        public override string ToString()
        {
            return Value?.FirstOrDefault()?.Value ?? "";
        }

        public override IEnumerable<string> ToStringEnumerable()
        {
            return Value?.Select(v => v.Value) ?? new string[] { };
        }
    }
}
