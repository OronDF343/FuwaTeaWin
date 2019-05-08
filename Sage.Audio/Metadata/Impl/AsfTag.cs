using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class AsfTag : MetadataBase
    {
        public AsfTag(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode) { }

        public override IDictionary<string, IList<string>> ExtendedFields { get; } = new Dictionary<string, IList<string>>();

        // https://docs.microsoft.com/en-us/windows/desktop/wmformat/id3-tag-support
        // https://docs.microsoft.com/en-us/windows/desktop/wmformat/attributes-with-multiple-values

        public override IListField Artist { get; } = new BasicListField();
        public override ITextField Copyright { get; } = new BasicTextField();
        public override ITextField WwwCopyright { get; } = new BasicTextField();
        public override IListField Comment { get; } = new BasicListField(1);
        public override INumericField Length { get; } = new BasicNumericField();
        public override ITextField Title { get; } = new BasicTextField();
        public override ITextField AlbumArtist { get; } = new BasicTextField(); // Can have multiple values!
        public override ITextField AlbumSort { get; } = new BasicTextField();
        public override ITextField Album { get; } = new BasicTextField();
        public override IListField ArtistSort { get; } = new BasicListField(1);
        public override ITextField WwwAudioFile { get; } = new BasicTextField();
        public override ITextField WwwAudioSource { get; } = new BasicTextField();
        public override IListField WwwArtist { get; } = new BasicListField(1);
        public override INumericField Bpm { get; } = new BasicNumericField();
        public override IListField Composer { get; } = new BasicListField();
        public override ITextField Conductor { get; } = new BasicTextField(); // Can have multiple values!
        public override ITextField ContentGroup { get; } = new BasicTextField();
        public override ITextField EncodedBy { get; } = new BasicTextField();
        public override ITextField EncoderSettings { get; } = new BasicTextField();
        public override IDateTimeField EncodingTime { get; } = new FileTimeField();
        public override IListField Genre { get; } = new BasicListField();
        public override IEnumField<MusicalKey> InitialKey { get; } = new MusicalKeyField();
        public override ITextField Isrc { get; } = new BasicTextField();
        public override ITextField Language { get; } = new BasicTextField(); // Can have multiple values!
        public override ITextField MixArtist { get; } = new BasicTextField();
        public override ITextField Mood { get; } = new BasicTextField(); // Can have multiple values!
        public override ITextField OrigAlbum { get; } = new BasicTextField();
        public override ITextField OrigArtist { get; } = new BasicTextField();
        public override ITextField OrigFileName { get; } = new BasicTextField();
        public override ITextField OrigLyricist { get; } = new BasicTextField();
        public override IDateTimeField OrigReleaseTime { get; } = new BasicDateTimeField(); // No strict format
        public override INumericField Disc { get; } = new BasicNumericField();
        public override INumericField DiscCount { get; } = new BasicNumericField();
        public override IList<IPicture> Picture { get; } = new List<IPicture>();
        public override ITextField Publisher { get; } = new BasicTextField();
        public override ITextField NetRadioStation { get; } = new BasicTextField();
        public override ITextField NetRadioOwner { get; } = new BasicTextField();
        public override ITextField SetSubtitle { get; } = new BasicTextField();
        public override ITextField Subtitle { get; } = new BasicTextField();
        public override ITextField TitleSort { get; } = new BasicTextField();
        public override INumericField Track { get; } = new BasicNumericField();
        public override IListField Lyricist { get; } = new BasicListField();
        public override IDateTimeField Year { get; } = new BasicDateTimeField(); // No strict format
    }
}
