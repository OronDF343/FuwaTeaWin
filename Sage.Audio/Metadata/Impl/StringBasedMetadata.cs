using System.Collections.Generic;
using System.Linq;
using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    /// <summary>
    /// Shared implementation for text-based formats such as Vorbis Comment, APE, ...
    /// </summary>
    public class StringBasedMetadata : MetadataBase
    {
        public StringBasedMetadata(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode)
        {
            WwwRadioPage = new ExtendedTextField(this, nameof(WwwRadioPage).ToUpperInvariant());
            WwwPublisher = new ExtendedTextField(this, nameof(WwwPublisher).ToUpperInvariant());
            WwwPayment = new ExtendedTextField(this, nameof(WwwPayment).ToUpperInvariant());
            WwwCopyright = new ExtendedTextField(this, nameof(WwwCopyright).ToUpperInvariant());
            WwwCommercialInfo = new ExtendedListField(this, nameof(WwwCommercialInfo).ToUpperInvariant());
            WwwAudioSource = new ExtendedTextField(this, nameof(WwwAudioSource).ToUpperInvariant());
            WwwAudioFile = new ExtendedTextField(this, nameof(WwwAudioFile).ToUpperInvariant());
            WwwArtist = new ExtendedListField(this, nameof(WwwArtist).ToUpperInvariant());
            UnSyncedLyrics = new ExtendedListField(this, nameof(UnSyncedLyrics).ToUpperInvariant());
            TaggingTime = new ExtendedDateTimeField(this, nameof(TaggingTime).ToUpperInvariant());
            Subtitle = new ExtendedTextField(this, nameof(Subtitle).ToUpperInvariant());
            SetSubtitle = new ExtendedTextField(this, nameof(SetSubtitle).ToUpperInvariant());
            ReleaseTime = new ExtendedDateTimeField(this, nameof(ReleaseTime).ToUpperInvariant());
            Rating_Wmp = new ExtendedNumericField(this, nameof(Rating_Wmp).ToUpperInvariant());
            Rating_WinAmp = new ExtendedNumericField(this, nameof(Rating_WinAmp).ToUpperInvariant());
            Rating_Mm = new ExtendedNumericField(this, nameof(Rating_Mm).ToUpperInvariant());
            Publisher = new ExtendedTextField(this, nameof(Publisher).ToUpperInvariant());
            OrigReleaseTime = new ExtendedDateTimeField(this, nameof(OrigReleaseTime).ToUpperInvariant());
            OrigLyricist = new ExtendedTextField(this, nameof(OrigLyricist).ToUpperInvariant());
            OrigFileName = new ExtendedTextField(this, nameof(OrigFileName).ToUpperInvariant());
            OrigArtist = new ExtendedTextField(this, nameof(OrigArtist).ToUpperInvariant());
            OrigAlbum = new ExtendedTextField(this, nameof(OrigAlbum).ToUpperInvariant());
            NetRadioStation = new ExtendedTextField(this, nameof(NetRadioStation).ToUpperInvariant());
            NetRadioOwner = new ExtendedTextField(this, nameof(NetRadioOwner).ToUpperInvariant());
            MusicianCredits = new ExtendedListField(this, nameof(MusicianCredits).ToUpperInvariant());
            MovementTotal = new ExtendedNumericField(this, nameof(MovementTotal).ToUpperInvariant());
            Movement = new ExtendedNumericField(this, nameof(Movement).ToUpperInvariant());
            MovementName = new ExtendedTextField(this, nameof(MovementName).ToUpperInvariant());
            Mood = new ExtendedTextField(this, nameof(Mood).ToUpperInvariant());
            MixArtist = new ExtendedTextField(this, nameof(MixArtist).ToUpperInvariant());
            MediaType = new ExtendedTextField(this, nameof(MediaType).ToUpperInvariant());
            Lyricist = new ExtendedListField(this, nameof(Lyricist).ToUpperInvariant());
            Length = new ExtendedNumericField(this, nameof(Length).ToUpperInvariant());
            Language = new ExtendedTextField(this, nameof(Language).ToUpperInvariant());
            Isrc = new ExtendedTextField(this, nameof(Isrc).ToUpperInvariant());
            InvolvedPeople = new ExtendedListField(this, nameof(InvolvedPeople).ToUpperInvariant());
            InitialKey = new ExtendedMusicalKeyField(this, nameof(InitialKey).ToUpperInvariant());
            Grouping = new ExtendedTextField(this, nameof(Grouping).ToUpperInvariant());
            FileOwner = new ExtendedTextField(this, nameof(FileOwner).ToUpperInvariant());
            EncodingTime = new ExtendedDateTimeField(this, nameof(EncodingTime).ToUpperInvariant());
            EncoderSettings = new ExtendedTextField(this, nameof(EncoderSettings).ToUpperInvariant());
            EncodedBy = new ExtendedTextField(this, nameof(EncodedBy).ToUpperInvariant());
            Copyright = new ExtendedTextField(this, nameof(Copyright).ToUpperInvariant());
            ContentGroup = new ExtendedTextField(this, nameof(ContentGroup).ToUpperInvariant());
            Conductor = new ExtendedTextField(this, nameof(Conductor).ToUpperInvariant());
            Bpm = new ExtendedNumericField(this, nameof(Bpm).ToUpperInvariant());
            DiscCount = new ExtendedNumericField(this, nameof(DiscCount).ToUpperInvariant());
            Disc = new ExtendedNumericField(this, nameof(Disc).ToUpperInvariant());
            ComposerSort = new ExtendedListField(this, nameof(ComposerSort).ToUpperInvariant());
            Composer = new ExtendedListField(this, nameof(Composer).ToUpperInvariant());
            AlbumArtistSort = new ExtendedTextField(this, nameof(AlbumArtistSort).ToUpperInvariant());
            AlbumArtist = new ExtendedListField(this, nameof(AlbumArtist).ToUpperInvariant());
            Comment = new ExtendedListField(this, nameof(Comment).ToUpperInvariant());
            Genre = new ExtendedListField(this, nameof(Genre).ToUpperInvariant());
            TrackCount = new ExtendedNumericField(this, nameof(TrackCount).ToUpperInvariant());
            Track = new ExtendedNumericField(this, nameof(Track).ToUpperInvariant());
            Year = new ExtendedDateTimeField(this, nameof(Year).ToUpperInvariant());
            AlbumSort = new ExtendedTextField(this, nameof(AlbumSort).ToUpperInvariant());
            Album = new ExtendedTextField(this, nameof(Album).ToUpperInvariant());
            ArtistSort = new ExtendedListField(this, nameof(ArtistSort).ToUpperInvariant());
            Artist = new ExtendedListField(this, nameof(Artist).ToUpperInvariant());
            TitleSort = new ExtendedTextField(this, nameof(TitleSort).ToUpperInvariant());
            Title = new ExtendedTextField(this, nameof(Title).ToUpperInvariant());
        }

        public override IDictionary<string, IMetadataField> FieldsByInternalId =>
            ExtendedFields.ToDictionary(g => g.Key, g => (IMetadataField)new BasicListField { Value = g.Value });

        public override IDictionary<string, IList<string>> ExtendedFields { get; } = new Dictionary<string, IList<string>>();

        public override IList<IPicture> Picture { get; } = new List<IPicture>();

        public override ITextField Title { get; }
        
        public override ITextField TitleSort { get; }

        public override IListField Artist { get; }

        public override IListField ArtistSort { get; }

        public override ITextField Album { get; }

        public override ITextField AlbumSort { get; }

        public override IDateTimeField Year { get; }

        public override INumericField Track { get; }

        public override INumericField TrackCount { get; }

        public override IListField Genre { get; }

        public override IListField Comment { get; }

        public override IListField AlbumArtist { get; }

        public override ITextField AlbumArtistSort { get; }

        public override IListField Composer { get; }

        public override IListField ComposerSort { get; }

        public override INumericField Disc { get; }

        public override INumericField DiscCount { get; }

        public override INumericField Bpm { get; }

        public override ITextField Conductor { get; }

        public override ITextField ContentGroup { get; }

        public override ITextField Copyright { get; }

        public override ITextField EncodedBy { get; }

        public override ITextField EncoderSettings { get; }

        public override IDateTimeField EncodingTime { get; }

        public override ITextField FileOwner { get; }

        public override ITextField Grouping { get; }

        public override IEnumField<MusicalKey> InitialKey { get; }

        public override IListField InvolvedPeople { get; }

        public override ITextField Isrc { get; }

        public override ITextField Language { get; }

        public override INumericField Length { get; }

        public override IListField Lyricist { get; }

        public override ITextField MediaType { get; }

        public override ITextField MixArtist { get; }

        public override ITextField Mood { get; }

        public override ITextField MovementName { get; }

        public override INumericField Movement { get; }

        public override INumericField MovementTotal { get; }

        public override IListField MusicianCredits { get; }

        public override ITextField NetRadioOwner { get; }

        public override ITextField NetRadioStation { get; }

        public override ITextField OrigAlbum { get; }

        public override ITextField OrigArtist { get; }

        public override ITextField OrigFileName { get; }

        public override ITextField OrigLyricist { get; }

        public override IDateTimeField OrigReleaseTime { get; }

        public override ITextField Publisher { get; }

        public override INumericField Rating_Mm { get; }

        public override INumericField Rating_WinAmp { get; }

        public override INumericField Rating_Wmp { get; }

        public override IDateTimeField ReleaseTime { get; }

        public override ITextField SetSubtitle { get; }

        public override ITextField Subtitle { get; }

        public override IDateTimeField TaggingTime { get; }

        public override IListField UnSyncedLyrics { get; }

        public override IListField WwwArtist { get; }

        public override ITextField WwwAudioFile { get; }

        public override ITextField WwwAudioSource { get; }

        public override IListField WwwCommercialInfo { get; }

        public override ITextField WwwCopyright { get; }

        public override ITextField WwwPayment { get; }

        public override ITextField WwwPublisher { get; }

        public override ITextField WwwRadioPage { get; }
    }
}
