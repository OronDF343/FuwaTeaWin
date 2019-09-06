using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    /// <summary>
    /// Shared implementation for text-based formats such as VorbisComment / APE / etc.
    /// </summary>
    public class StringBasedMetadata : MetadataBase
    {
        public StringBasedMetadata(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode)
        {
            foreach (var field in CommonFieldIds.All)
                FieldsById.Add(field, new BasicListField());
        }

        public override MetadataField AddCustomField(string key)
        {
            var f = new BasicListField();
            FieldsById.Add(key, f);
            return f;
        }

        public override IList<IPicture> Picture { get; } = new List<IPicture>();
    }
}
