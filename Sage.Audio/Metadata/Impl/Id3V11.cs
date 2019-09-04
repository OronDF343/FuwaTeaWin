using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class Id3V11 : MetadataBase
    {
        public Id3V11(bool isReadOnly = false)
            : base(isReadOnly, false, false)
        {
            FieldsById = new Dictionary<string, IMetadataField>
            {
                { CommonFieldIds.Title, new BasicTextField(30) },
                { CommonFieldIds.Artist, new JoinedListField(";", 30) },
                { CommonFieldIds.Album, new BasicTextField(30) },
                { CommonFieldIds.Year, new BasicDateTimeField(1) },
                { CommonFieldIds.Comment, new BasicTextField(28) },
                { CommonFieldIds.Track, new BasicNumericField(255) },
                { CommonFieldIds.Genre, new Id3V1GenreField() }
            };
        }
    }
}
