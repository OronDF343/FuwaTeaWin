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
        IReadOnlyDictionary<string, MetadataField> Fields { get; }

        /// <summary>
        /// If supported, adds a custom field.
        /// </summary>
        /// <remarks>
        /// The return value should be of type <see cref="ITextField"/> or <see cref="IListField"/>, depending on the format.
        /// Example: The former is used for ID3v2, the latter for VorbisComment / APE / etc.
        /// </remarks>
        /// <param name="key">The key of the field.</param>
        /// <returns>The newly-crated field.</returns>
        MetadataField AddCustomField(string key);

        // Special field - May not be part of the tag itself (as is the case with XiphComment)
        IList<IPicture> Picture { get; }

        // Mp3tag basic tags

        MetadataField Title { get; }
        MetadataField TitleSort { get; }
        MetadataField Artist { get; }
        MetadataField ArtistSort { get; }
        MetadataField Album { get; }
        MetadataField AlbumSort { get; }
        MetadataField Year { get; }
        MetadataField Track { get; }
        MetadataField TrackCount { get; }
        MetadataField Genre { get; }
        MetadataField Comment { get; }
        MetadataField AlbumArtist { get; }
        MetadataField AlbumArtistSort { get; }
        MetadataField Composer { get; }
        MetadataField ComposerSort { get; }
        MetadataField Disc { get; }
        MetadataField DiscCount { get; }

        // Mp3tag extended tags

        MetadataField Bpm { get; }
        MetadataField Conductor { get; }
        MetadataField ContentGroup { get; }
        MetadataField Copyright { get; }
        MetadataField EncodedBy { get; }
        MetadataField EncoderSettings { get; }
        MetadataField EncodingTime { get; }
        MetadataField FileOwner { get; }
        MetadataField Grouping { get; }
        MetadataField InitialKey { get; }
        MetadataField InvolvedPeople { get; }
        MetadataField Isrc { get; }
        MetadataField Language { get; }
        MetadataField Length { get; }
        MetadataField Lyricist { get; }
        MetadataField MediaType { get; }
        MetadataField MixArtist { get; }
        MetadataField Mood { get; }
        MetadataField MovementName { get; }
        MetadataField Movement { get; }
        MetadataField MovementTotal { get; }
        MetadataField MusicianCredits { get; }
        MetadataField NetRadioOwner { get; }
        MetadataField NetRadioStation { get; }
        MetadataField OrigAlbum { get; }
        MetadataField OrigArtist { get; }
        MetadataField OrigFileName { get; }
        MetadataField OrigLyricist { get; }
        MetadataField OrigReleaseTime { get; }
        MetadataField Publisher { get; }
        MetadataField Rating_Mm { get; }
        MetadataField Rating_WinAmp { get; }
        MetadataField Rating_Wmp { get; }
        MetadataField ReleaseTime { get; }
        MetadataField SetSubtitle { get; }
        MetadataField Subtitle { get; }
        MetadataField TaggingTime { get; }
        MetadataField UnsyncedLyrics { get; }
        MetadataField WwwArtist { get; }
        MetadataField WwwAudioFile { get; }
        MetadataField WwwAudioSource { get; }
        MetadataField WwwCommercialInfo { get; }
        MetadataField WwwCopyright { get; }
        MetadataField WwwPayment { get; }
        MetadataField WwwPublisher { get; }
        MetadataField WwwRadioPage { get; }
    }
}
