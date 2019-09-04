using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class RiffInfo : MetadataBase
    {
        public RiffInfo(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode, false)
        {
            // TODO: These are supported by TagLib#. Determine if there are others we can add support for.
            // TODO: Custom tags technically supported, but only with 4-character alphanumeric IDs
            // TODO: Pictures?
            FieldsById = new Dictionary<string, IMetadataField>
            {
                { CommonFieldIds.Title, new BasicTextField() },
                { CommonFieldIds.Artist, new BasicListField() },
                { CommonFieldIds.AlbumArtist, new BasicListField() },
                { CommonFieldIds.Year, new BasicDateTimeField(1) },
                { CommonFieldIds.Track, new BasicNumericField() },
                { CommonFieldIds.TrackCount, new BasicNumericField() },
                { CommonFieldIds.Genre, new BasicListField() },
                { CommonFieldIds.Comment, new BasicTextField() },
                { CommonFieldIds.Composer, new BasicListField() },
                { CommonFieldIds.Copyright, new BasicTextField() }
            };
        }
    }
}
