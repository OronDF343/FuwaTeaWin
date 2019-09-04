using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class AppleTag : MetadataBase
    {
        public AppleTag(bool isReadOnly = false)
            : base(isReadOnly, supportsCustomTags: false) // Forced UTF-8 without BOM
        {
            const string separator = "; ";
            FieldsById = new Dictionary<string, IMetadataField>
            {
                // From http://atomicparsley.sourceforge.net/mpeg-4files.html
                { CommonFieldIds.Album, new BasicTextField(255) },
                { CommonFieldIds.Artist, new JoinedListField(separator, 255) },
                { CommonFieldIds.AlbumArtist, new JoinedListField(separator, 255) },
                { CommonFieldIds.Comment, new BasicTextField(255) },
                { CommonFieldIds.Year, new BasicTextField(255) },
                { CommonFieldIds.Title, new BasicTextField(255) },
                // TODO: AppleGenreTag - Supports either single ID or text
                { CommonFieldIds.Genre, new JoinedListField(separator, 255) },
                { CommonFieldIds.Track, new BasicNumericField(255) },
                { CommonFieldIds.Disc, new BasicNumericField(255) },
                { CommonFieldIds.Composer, new JoinedListField(separator, 255) },
                { CommonFieldIds.EncodedBy, new BasicTextField(255) },
                { CommonFieldIds.Bpm, new BasicNumericField(255) },
                { CommonFieldIds.Copyright, new BasicTextField(255) },
                { CommonFieldIds.Grouping, new BasicTextField(255) },
                { CommonFieldIds.Subtitle, new BasicTextField(255) },
                // Only text field that allows more than 255 characters
                { CommonFieldIds.UnsyncedLyrics, new BasicTextField() },

                // Supported by TagLib
                { CommonFieldIds.TrackCount, new BasicNumericField(255) },
                { CommonFieldIds.DiscCount, new BasicNumericField(255) },

                // Additional fields http://help.mp3tag.de/main_tags.html
                { CommonFieldIds.AlbumSort, new BasicTextField(255) },
                { CommonFieldIds.AlbumArtistSort, new BasicTextField(255) },
                { CommonFieldIds.ArtistSort, new JoinedListField(separator, 255) },
                { CommonFieldIds.ComposerSort, new JoinedListField(separator, 255) },
                { CommonFieldIds.Conductor, new BasicTextField(255) },
                { CommonFieldIds.ContentGroup, new BasicTextField(255) },
                { CommonFieldIds.MovementName, new BasicTextField(255) },
                { CommonFieldIds.Movement, new BasicNumericField(255) },
                { CommonFieldIds.MovementTotal, new BasicNumericField(255) },
                { CommonFieldIds.TitleSort, new BasicTextField(255) }
            };
        }

        public override IList<IPicture> Picture { get; } = new List<IPicture>(); // Only field that supports more than 1 value. JPEG or PNG. No description or type.
    }
}
