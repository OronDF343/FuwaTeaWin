using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Lib.Collections;

namespace Sage.Audio.Metadata
{
    public abstract class MetadataBase : IMetadata
    {
        public MetadataBase(bool isReadOnly = false, bool supportsUnicode = true, bool supportsCustomTags = true)
        {
            IsReadOnly = isReadOnly;
            SupportsUnicode = supportsUnicode;
            SupportsCustomTags = supportsCustomTags;
        }

        public bool IsReadOnly { get; }
        public bool SupportsUnicode { get; }
        public bool SupportsCustomTags { get; }

        protected IDictionary<string, IMetadataField> FieldsById { get; set; } = new Dictionary<string, IMetadataField>();
        public IReadOnlyDictionary<string, IMetadataField> Fields => new ReadOnlyDictionary<string, IMetadataField>(FieldsById);
        public virtual IMetadataField AddCustomField(string key) => throw new NotSupportedException();

        public virtual IList<IPicture> Picture { get; } = null;

        public virtual IMetadataField Title => FieldsById.GetOrDefault(CommonFieldIds.Title);
        public virtual IMetadataField TitleSort => FieldsById.GetOrDefault(CommonFieldIds.TitleSort);
        public virtual IMetadataField Artist => FieldsById.GetOrDefault(CommonFieldIds.Artist);
        public virtual IMetadataField ArtistSort => FieldsById.GetOrDefault(CommonFieldIds.ArtistSort);
        public virtual IMetadataField Album => FieldsById.GetOrDefault(CommonFieldIds.Album);
        public virtual IMetadataField AlbumSort => FieldsById.GetOrDefault(CommonFieldIds.AlbumSort);
        public virtual IMetadataField Year => FieldsById.GetOrDefault(CommonFieldIds.Year);
        public virtual IMetadataField Track => FieldsById.GetOrDefault(CommonFieldIds.Track);
        public virtual IMetadataField TrackCount => FieldsById.GetOrDefault(CommonFieldIds.TrackCount);
        public virtual IMetadataField Genre => FieldsById.GetOrDefault(CommonFieldIds.Genre);
        public virtual IMetadataField Comment => FieldsById.GetOrDefault(CommonFieldIds.Comment);
        public virtual IMetadataField AlbumArtist => FieldsById.GetOrDefault(CommonFieldIds.AlbumArtist);
        public virtual IMetadataField AlbumArtistSort => FieldsById.GetOrDefault(CommonFieldIds.AlbumArtistSort);
        public virtual IMetadataField Composer => FieldsById.GetOrDefault(CommonFieldIds.Composer);
        public virtual IMetadataField ComposerSort => FieldsById.GetOrDefault(CommonFieldIds.ComposerSort);
        public virtual IMetadataField Disc => FieldsById.GetOrDefault(CommonFieldIds.Disc);
        public virtual IMetadataField DiscCount => FieldsById.GetOrDefault(CommonFieldIds.DiscCount);
        public virtual IMetadataField Bpm => FieldsById.GetOrDefault(CommonFieldIds.Bpm);
        public virtual IMetadataField Conductor => FieldsById.GetOrDefault(CommonFieldIds.Conductor);
        public virtual IMetadataField ContentGroup => FieldsById.GetOrDefault(CommonFieldIds.ContentGroup);
        public virtual IMetadataField Copyright => FieldsById.GetOrDefault(CommonFieldIds.Copyright);
        public virtual IMetadataField EncodedBy => FieldsById.GetOrDefault(CommonFieldIds.EncodedBy);
        public virtual IMetadataField EncoderSettings => FieldsById.GetOrDefault(CommonFieldIds.EncoderSettings);
        public virtual IMetadataField EncodingTime => FieldsById.GetOrDefault(CommonFieldIds.EncodingTime);
        public virtual IMetadataField FileOwner => FieldsById.GetOrDefault(CommonFieldIds.FileOwner);
        public virtual IMetadataField Grouping => FieldsById.GetOrDefault(CommonFieldIds.Grouping);
        public virtual IMetadataField InitialKey => FieldsById.GetOrDefault(CommonFieldIds.InitialKey);
        public virtual IMetadataField InvolvedPeople => FieldsById.GetOrDefault(CommonFieldIds.InvolvedPeople);
        public virtual IMetadataField Isrc => FieldsById.GetOrDefault(CommonFieldIds.Isrc);
        public virtual IMetadataField Language => FieldsById.GetOrDefault(CommonFieldIds.Language);
        public virtual IMetadataField Length => FieldsById.GetOrDefault(CommonFieldIds.Length);
        public virtual IMetadataField Lyricist => FieldsById.GetOrDefault(CommonFieldIds.Lyricist);
        public virtual IMetadataField MediaType => FieldsById.GetOrDefault(CommonFieldIds.MediaType);
        public virtual IMetadataField MixArtist => FieldsById.GetOrDefault(CommonFieldIds.MixArtist);
        public virtual IMetadataField Mood => FieldsById.GetOrDefault(CommonFieldIds.Mood);
        public virtual IMetadataField MovementName => FieldsById.GetOrDefault(CommonFieldIds.MovementName);
        public virtual IMetadataField Movement => FieldsById.GetOrDefault(CommonFieldIds.Movement);
        public virtual IMetadataField MovementTotal => FieldsById.GetOrDefault(CommonFieldIds.MovementTotal);
        public virtual IMetadataField MusicianCredits => FieldsById.GetOrDefault(CommonFieldIds.MusicianCredits);
        public virtual IMetadataField NetRadioOwner => FieldsById.GetOrDefault(CommonFieldIds.NetRadioOwner);
        public virtual IMetadataField NetRadioStation => FieldsById.GetOrDefault(CommonFieldIds.NetRadioStation);
        public virtual IMetadataField OrigAlbum => FieldsById.GetOrDefault(CommonFieldIds.OrigAlbum);
        public virtual IMetadataField OrigArtist => FieldsById.GetOrDefault(CommonFieldIds.OrigArtist);
        public virtual IMetadataField OrigFileName => FieldsById.GetOrDefault(CommonFieldIds.OrigFileName);
        public virtual IMetadataField OrigLyricist => FieldsById.GetOrDefault(CommonFieldIds.OrigLyricist);
        public virtual IMetadataField OrigReleaseTime => FieldsById.GetOrDefault(CommonFieldIds.OrigReleaseTime);
        public virtual IMetadataField Publisher => FieldsById.GetOrDefault(CommonFieldIds.Publisher);
        public virtual IMetadataField Rating_Mm => FieldsById.GetOrDefault(CommonFieldIds.Rating_Mm);
        public virtual IMetadataField Rating_WinAmp => FieldsById.GetOrDefault(CommonFieldIds.Rating_WinAmp);
        public virtual IMetadataField Rating_Wmp => FieldsById.GetOrDefault(CommonFieldIds.Rating_Wmp);
        public virtual IMetadataField ReleaseTime => FieldsById.GetOrDefault(CommonFieldIds.ReleaseTime);
        public virtual IMetadataField SetSubtitle => FieldsById.GetOrDefault(CommonFieldIds.SetSubtitle);
        public virtual IMetadataField Subtitle => FieldsById.GetOrDefault(CommonFieldIds.Subtitle);
        public virtual IMetadataField TaggingTime => FieldsById.GetOrDefault(CommonFieldIds.TaggingTime);
        public virtual IMetadataField UnsyncedLyrics => FieldsById.GetOrDefault(CommonFieldIds.UnsyncedLyrics);
        public virtual IMetadataField WwwArtist => FieldsById.GetOrDefault(CommonFieldIds.WwwArtist);
        public virtual IMetadataField WwwAudioFile => FieldsById.GetOrDefault(CommonFieldIds.WwwAudioFile);
        public virtual IMetadataField WwwAudioSource => FieldsById.GetOrDefault(CommonFieldIds.WwwAudioSource);
        public virtual IMetadataField WwwCommercialInfo => FieldsById.GetOrDefault(CommonFieldIds.WwwCommercialInfo);
        public virtual IMetadataField WwwCopyright => FieldsById.GetOrDefault(CommonFieldIds.WwwCopyright);
        public virtual IMetadataField WwwPayment => FieldsById.GetOrDefault(CommonFieldIds.WwwPayment);
        public virtual IMetadataField WwwPublisher => FieldsById.GetOrDefault(CommonFieldIds.WwwPublisher);
        public virtual IMetadataField WwwRadioPage => FieldsById.GetOrDefault(CommonFieldIds.WwwRadioPage);
    }
}
