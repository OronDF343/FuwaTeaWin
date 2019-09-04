using System.Collections.Generic;

namespace Sage.Audio.Metadata
{
    public interface IMetadata
    {
        // Used mostly for streams
        bool IsReadOnly { get; }
        // Used for old kinds of tags
        bool SupportsUnicode { get; }
        // Supports adding custom fields
        bool SupportsCustomTags { get; }
        
        /// <summary>
        /// Gets a dictionary of all existing fields.
        /// </summary>
        /// <remarks>
        /// See <see cref="CommonFieldIds"/> for the built-in keys available.
        /// In ID3v2, TXXX frames are keyed by description, while TODO: WXXX frames are keyed by "WXXX:" + description?
        /// </remarks>
        IReadOnlyDictionary<string, IMetadataField> Fields { get; }

        /// <summary>
        /// If supported, adds a custom field.
        /// </summary>
        /// <remarks>
        /// The return value should be of type <see cref="ITextField"/> or <see cref="IListField"/>, depending on the format.
        /// Example: The former is used for ID3v2, the latter for VorbisComment / APE / etc.
        /// </remarks>
        /// <param name="key">The key of the field.</param>
        /// <returns>The newly-crated field.</returns>
        IMetadataField AddCustomField(string key);

        // Special field - May not be part of the tag itself (as is the case with XiphComment)
        IList<IPicture> Picture { get; }

        // Mp3tag basic tags

        IMetadataField Title { get; }
        IMetadataField TitleSort { get; }
        IMetadataField Artist { get; }
        IMetadataField ArtistSort { get; }
        IMetadataField Album { get; }
        IMetadataField AlbumSort { get; }
        IMetadataField Year { get; }
        IMetadataField Track { get; }
        IMetadataField TrackCount { get; }
        IMetadataField Genre { get; }
        IMetadataField Comment { get; }
        IMetadataField AlbumArtist { get; }
        IMetadataField AlbumArtistSort { get; }
        IMetadataField Composer { get; }
        IMetadataField ComposerSort { get; }
        IMetadataField Disc { get; }
        IMetadataField DiscCount { get; }

        // Mp3tag extended tags

        IMetadataField Bpm { get; }
        IMetadataField Conductor { get; }
        IMetadataField ContentGroup { get; }
        IMetadataField Copyright { get; }
        IMetadataField EncodedBy { get; }
        IMetadataField EncoderSettings { get; }
        IMetadataField EncodingTime { get; }
        IMetadataField FileOwner { get; }
        IMetadataField Grouping { get; }
        IMetadataField InitialKey { get; }
        IMetadataField InvolvedPeople { get; }
        IMetadataField Isrc { get; }
        IMetadataField Language { get; }
        IMetadataField Length { get; }
        IMetadataField Lyricist { get; }
        IMetadataField MediaType { get; }
        IMetadataField MixArtist { get; }
        IMetadataField Mood { get; }
        IMetadataField MovementName { get; }
        IMetadataField Movement { get; }
        IMetadataField MovementTotal { get; }
        IMetadataField MusicianCredits { get; }
        IMetadataField NetRadioOwner { get; }
        IMetadataField NetRadioStation { get; }
        IMetadataField OrigAlbum { get; }
        IMetadataField OrigArtist { get; }
        IMetadataField OrigFileName { get; }
        IMetadataField OrigLyricist { get; }
        IMetadataField OrigReleaseTime { get; }
        IMetadataField Publisher { get; }
        IMetadataField Rating_Mm { get; }
        IMetadataField Rating_WinAmp { get; }
        IMetadataField Rating_Wmp { get; }
        IMetadataField ReleaseTime { get; }
        IMetadataField SetSubtitle { get; }
        IMetadataField Subtitle { get; }
        IMetadataField TaggingTime { get; }
        IMetadataField UnsyncedLyrics { get; }
        IMetadataField WwwArtist { get; }
        IMetadataField WwwAudioFile { get; }
        IMetadataField WwwAudioSource { get; }
        IMetadataField WwwCommercialInfo { get; }
        IMetadataField WwwCopyright { get; }
        IMetadataField WwwPayment { get; }
        IMetadataField WwwPublisher { get; }
        IMetadataField WwwRadioPage { get; }
    }
}
