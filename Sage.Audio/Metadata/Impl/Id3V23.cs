using System.Collections.Generic;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class Id3V23 : MetadataBase
    {
        public Id3V23(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode) { }
        
        public override IDictionary<string, IList<string>> ExtendedFields { get; } = new Dictionary<string, IList<string>>();

        public override IList<IPicture> Picture { get; } = new List<IPicture>();

        public override ITextField Title { get; } = new BasicTextField();

        public override ITextField TitleSort { get; } = new BasicTextField();

        public override IListField Artist { get; } = new JoinedListField();

        public override IListField ArtistSort { get; } = new JoinedListField();

        public override ITextField Album { get; } = new BasicTextField();

        public override ITextField AlbumSort { get; } = new BasicTextField();

        public override IDateTimeField Year { get; } = new BasicDateTimeField(5);

        public override INumericField Track { get; } = new BasicNumericField();

        public override INumericField TrackCount { get; } = new BasicNumericField();

        public override IListField Genre { get; } = new JoinedListField(); // TODO: Genre index support

        public override IListField Comment { get; } = new BasicListField(); // TODO: Content descriptors?

        public override IListField AlbumArtist { get; } = new BasicListField();

        public override ITextField AlbumArtistSort { get; } = new BasicTextField();

        public override IListField Composer { get; } = new JoinedListField();

        public override IListField ComposerSort { get; } = new JoinedListField();

        public override INumericField Disc { get; } = new BasicNumericField();

        public override INumericField DiscCount { get; } = new BasicNumericField();

        public override INumericField Bpm { get; } = new BasicNumericField();

        public override ITextField Conductor { get; } = new BasicTextField();

        public override ITextField ContentGroup { get; } = new BasicTextField();

        public override ITextField Copyright { get; } = new BasicTextField();

        public override ITextField EncodedBy { get; } = new BasicTextField();

        public override ITextField EncoderSettings { get; } = new BasicTextField();
        
        public override ITextField FileOwner { get; } = new BasicTextField();

        public override ITextField Grouping { get; } = new BasicTextField();

        public override IEnumField<MusicalKey> InitialKey { get; } = new MusicalKeyField();

        public override IListField InvolvedPeople { get; } = new BasicListField();

        public override ITextField Isrc { get; } = new BasicTextField();

        public override ITextField Language { get; } = new BasicTextField();

        public override INumericField Length { get; } = new BasicNumericField();

        public override IListField Lyricist { get; } = new JoinedListField();

        public override ITextField MediaType { get; } = new BasicTextField();

        public override ITextField MixArtist { get; } = new BasicTextField();
        
        public override ITextField MovementName { get; } = new BasicTextField();

        public override INumericField Movement { get; } = new BasicNumericField();

        public override INumericField MovementTotal { get; } = new BasicNumericField();

        public override ITextField NetRadioOwner { get; } = new BasicTextField();

        public override ITextField NetRadioStation { get; } = new BasicTextField();

        public override ITextField OrigAlbum { get; } = new BasicTextField();

        public override ITextField OrigArtist { get; } = new BasicTextField();

        public override ITextField OrigFileName { get; } = new BasicTextField();

        public override ITextField OrigLyricist { get; } = new BasicTextField();

        public override IDateTimeField OrigReleaseTime { get; } = new BasicDateTimeField(1);

        public override ITextField Publisher { get; } = new BasicTextField();

        public override INumericField Rating_Mm { get; } = new BasicNumericField();

        public override INumericField Rating_WinAmp { get; } = new BasicNumericField();

        public override INumericField Rating_Wmp { get; } = new BasicNumericField();
        
        public override ITextField Subtitle { get; } = new BasicTextField();
        
        public override IListField UnSyncedLyrics { get; } = new BasicListField();

        public override IListField WwwArtist { get; } = new BasicListField();

        public override ITextField WwwAudioFile { get; } = new BasicTextField();

        public override ITextField WwwAudioSource { get; } = new BasicTextField();

        public override IListField WwwCommercialInfo { get; } = new BasicListField();

        public override ITextField WwwCopyright { get; } = new BasicTextField();

        public override ITextField WwwPayment { get; } = new BasicTextField();

        public override ITextField WwwPublisher { get; } = new BasicTextField();

        public override ITextField WwwRadioPage { get; } = new BasicTextField();
    }
}
