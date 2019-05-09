using System.Collections.Generic;

namespace Sage.Audio.Metadata
{
    public abstract class MetadataBase : IMetadata
    {
        public MetadataBase(bool isReadOnly = false, bool supportsUnicode = true)
        {
            IsReadOnly = isReadOnly;
            SupportsUnicode = supportsUnicode;
        }

        public bool IsReadOnly { get; }
        public bool SupportsUnicode { get; }
        public virtual IDictionary<string, IMetadataField> FieldsByInternalId { get; } = null;
        public virtual IDictionary<string, IList<string>> ExtendedFields { get; } = null;
        public virtual IList<IPicture> Picture { get; } = null;
        public virtual ITextField Title { get; } = null;
        public virtual ITextField TitleSort { get; } = null;
        public virtual IListField Artist { get; } = null;
        public virtual IListField ArtistSort { get; } = null;
        public virtual ITextField Album { get; } = null;
        public virtual ITextField AlbumSort { get; } = null;
        public virtual IDateTimeField Year { get; } = null;
        public virtual INumericField Track { get; } = null;
        public virtual INumericField TrackCount { get; } = null;
        public virtual IListField Genre { get; } = null;
        public virtual IListField Comment { get; } = null;
        public virtual IListField AlbumArtist { get; } = null;
        public virtual ITextField AlbumArtistSort { get; } = null;
        public virtual IListField Composer { get; } = null;
        public virtual IListField ComposerSort { get; } = null;
        public virtual INumericField Disc { get; } = null;
        public virtual INumericField DiscCount { get; } = null;
        public virtual INumericField Bpm { get; } = null;
        public virtual ITextField Conductor { get; } = null;
        public virtual ITextField ContentGroup { get; } = null;
        public virtual ITextField Copyright { get; } = null;
        public virtual ITextField EncodedBy { get; } = null;
        public virtual ITextField EncoderSettings { get; } = null;
        public virtual IDateTimeField EncodingTime { get; } = null;
        public virtual ITextField FileOwner { get; } = null;
        public virtual ITextField Grouping { get; } = null;
        public virtual IEnumField<MusicalKey> InitialKey { get; } = null;
        public virtual IListField InvolvedPeople { get; } = null;
        public virtual ITextField Isrc { get; } = null;
        public virtual ITextField Language { get; } = null;
        public virtual INumericField Length { get; } = null;
        public virtual IListField Lyricist { get; } = null;
        public virtual ITextField MediaType { get; } = null;
        public virtual ITextField MixArtist { get; } = null;
        public virtual ITextField Mood { get; } = null;
        public virtual ITextField MovementName { get; } = null;
        public virtual INumericField Movement { get; } = null;
        public virtual INumericField MovementTotal { get; } = null;
        public virtual IListField MusicianCredits { get; } = null;
        public virtual ITextField NetRadioOwner { get; } = null;
        public virtual ITextField NetRadioStation { get; } = null;
        public virtual ITextField OrigAlbum { get; } = null;
        public virtual ITextField OrigArtist { get; } = null;
        public virtual ITextField OrigFileName { get; } = null;
        public virtual ITextField OrigLyricist { get; } = null;
        public virtual IDateTimeField OrigReleaseTime { get; } = null;
        public virtual ITextField Publisher { get; } = null;
        public virtual INumericField Rating_Mm { get; } = null;
        public virtual INumericField Rating_WinAmp { get; } = null;
        public virtual INumericField Rating_Wmp { get; } = null;
        public virtual IDateTimeField ReleaseTime { get; } = null;
        public virtual ITextField SetSubtitle { get; } = null;
        public virtual ITextField Subtitle { get; } = null;
        public virtual IDateTimeField TaggingTime { get; } = null;
        public virtual IListField UnSyncedLyrics { get; } = null;
        public virtual IListField WwwArtist { get; } = null;
        public virtual ITextField WwwAudioFile { get; } = null;
        public virtual ITextField WwwAudioSource { get; } = null;
        public virtual IListField WwwCommercialInfo { get; } = null;
        public virtual ITextField WwwCopyright { get; } = null;
        public virtual ITextField WwwPayment { get; } = null;
        public virtual ITextField WwwPublisher { get; } = null;
        public virtual ITextField WwwRadioPage { get; } = null;
    }
}
