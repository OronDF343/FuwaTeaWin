using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class AsfTag : MetadataBase
    {
        public AsfTag(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode, false)
        {
            // https://docs.microsoft.com/en-us/windows/desktop/wmformat/id3-tag-support
            // https://docs.microsoft.com/en-us/windows/desktop/wmformat/attributes-with-multiple-values
            FieldsById = new Dictionary<string, IMetadataField>
            {
                { CommonFieldIds.Artist, new BasicListField() },
                { CommonFieldIds.Copyright, new BasicTextField() },
                { CommonFieldIds.WwwCopyright, new BasicTextField() },
                { CommonFieldIds.Comment, new BasicTextField() },
                { CommonFieldIds.Length, new BasicNumericField() },
                { CommonFieldIds.Title, new BasicTextField() },
                { CommonFieldIds.AlbumArtist, new BasicListField() },
                { CommonFieldIds.AlbumSort, new BasicTextField() },
                { CommonFieldIds.Album, new BasicTextField() },
                { CommonFieldIds.ArtistSort, new BasicTextField() },
                { CommonFieldIds.WwwAudioFile, new BasicTextField() },
                { CommonFieldIds.WwwAudioSource, new BasicTextField() },
                { CommonFieldIds.WwwArtist, new BasicTextField() },
                { CommonFieldIds.Bpm, new BasicNumericField() },
                { CommonFieldIds.Composer, new BasicListField() },
                { CommonFieldIds.Conductor, new BasicListField() },
                { CommonFieldIds.ContentGroup, new BasicTextField() },
                { CommonFieldIds.EncodedBy, new BasicTextField() },
                { CommonFieldIds.EncoderSettings, new BasicTextField() },
                { CommonFieldIds.EncodingTime, new FileTimeField() },
                { CommonFieldIds.Genre, new BasicListField() },
                { CommonFieldIds.InitialKey, new MusicalKeyField() },
                { CommonFieldIds.Isrc, new BasicTextField() },
                { CommonFieldIds.Language, new BasicListField() },
                { CommonFieldIds.MixArtist, new BasicTextField() },
                { CommonFieldIds.Mood, new BasicListField() },
                { CommonFieldIds.OrigAlbum, new BasicTextField() },
                { CommonFieldIds.OrigArtist, new BasicTextField() },
                { CommonFieldIds.OrigFileName, new BasicTextField() },
                { CommonFieldIds.OrigLyricist, new BasicTextField() },
                { CommonFieldIds.OrigReleaseTime, new BasicTextField() },
                { CommonFieldIds.Disc, new BasicNumericField() },
                { CommonFieldIds.DiscCount, new BasicNumericField() },
                { CommonFieldIds.Publisher, new BasicTextField() },
                { CommonFieldIds.NetRadioStation, new BasicTextField() },
                { CommonFieldIds.NetRadioOwner, new BasicTextField() },
                { CommonFieldIds.SetSubtitle, new BasicTextField() },
                { CommonFieldIds.Subtitle, new BasicTextField() },
                { CommonFieldIds.TitleSort, new BasicTextField() },
                { CommonFieldIds.Track, new BasicNumericField() },
                { CommonFieldIds.Lyricist, new BasicListField() },
                { CommonFieldIds.Year, new BasicTextField() }
            };
        }
        
        public override IList<IPicture> Picture { get; } = new List<IPicture>();
    }
}
