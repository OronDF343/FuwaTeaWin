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

        protected IDictionary<string, MetadataField> FieldsById { get; set; } = new Dictionary<string, MetadataField>();
        public IReadOnlyDictionary<string, MetadataField> Fields => new ReadOnlyDictionary<string, MetadataField>(FieldsById);
        public virtual MetadataField AddCustomField(string key) => throw new NotSupportedException();

        public virtual IList<IPicture> Picture { get; } = null;

        public virtual MetadataField Title => FieldsById.GetOrDefault(CommonFieldIds.Title);
        public virtual MetadataField TitleSort => FieldsById.GetOrDefault(CommonFieldIds.TitleSort);
        public virtual MetadataField Artist => FieldsById.GetOrDefault(CommonFieldIds.Artist);
        public virtual MetadataField ArtistSort => FieldsById.GetOrDefault(CommonFieldIds.ArtistSort);
        public virtual MetadataField Album => FieldsById.GetOrDefault(CommonFieldIds.Album);
        public virtual MetadataField AlbumSort => FieldsById.GetOrDefault(CommonFieldIds.AlbumSort);
        public virtual MetadataField Year => FieldsById.GetOrDefault(CommonFieldIds.Year);
        public virtual MetadataField Track => FieldsById.GetOrDefault(CommonFieldIds.Track);
        public virtual MetadataField TrackCount => FieldsById.GetOrDefault(CommonFieldIds.TrackCount);
        public virtual MetadataField Genre => FieldsById.GetOrDefault(CommonFieldIds.Genre);
        public virtual MetadataField Comment => FieldsById.GetOrDefault(CommonFieldIds.Comment);
        public virtual MetadataField AlbumArtist => FieldsById.GetOrDefault(CommonFieldIds.AlbumArtist);
        public virtual MetadataField AlbumArtistSort => FieldsById.GetOrDefault(CommonFieldIds.AlbumArtistSort);
        public virtual MetadataField Composer => FieldsById.GetOrDefault(CommonFieldIds.Composer);
        public virtual MetadataField ComposerSort => FieldsById.GetOrDefault(CommonFieldIds.ComposerSort);
        public virtual MetadataField Disc => FieldsById.GetOrDefault(CommonFieldIds.Disc);
        public virtual MetadataField DiscCount => FieldsById.GetOrDefault(CommonFieldIds.DiscCount);
        public virtual MetadataField Bpm => FieldsById.GetOrDefault(CommonFieldIds.Bpm);
        public virtual MetadataField Conductor => FieldsById.GetOrDefault(CommonFieldIds.Conductor);
        public virtual MetadataField ContentGroup => FieldsById.GetOrDefault(CommonFieldIds.ContentGroup);
        public virtual MetadataField Copyright => FieldsById.GetOrDefault(CommonFieldIds.Copyright);
        public virtual MetadataField EncodedBy => FieldsById.GetOrDefault(CommonFieldIds.EncodedBy);
        public virtual MetadataField EncoderSettings => FieldsById.GetOrDefault(CommonFieldIds.EncoderSettings);
        public virtual MetadataField EncodingTime => FieldsById.GetOrDefault(CommonFieldIds.EncodingTime);
        public virtual MetadataField FileOwner => FieldsById.GetOrDefault(CommonFieldIds.FileOwner);
        public virtual MetadataField Grouping => FieldsById.GetOrDefault(CommonFieldIds.Grouping);
        public virtual MetadataField InitialKey => FieldsById.GetOrDefault(CommonFieldIds.InitialKey);
        public virtual MetadataField InvolvedPeople => FieldsById.GetOrDefault(CommonFieldIds.InvolvedPeople);
        public virtual MetadataField Isrc => FieldsById.GetOrDefault(CommonFieldIds.Isrc);
        public virtual MetadataField Language => FieldsById.GetOrDefault(CommonFieldIds.Language);
        public virtual MetadataField Length => FieldsById.GetOrDefault(CommonFieldIds.Length);
        public virtual MetadataField Lyricist => FieldsById.GetOrDefault(CommonFieldIds.Lyricist);
        public virtual MetadataField MediaType => FieldsById.GetOrDefault(CommonFieldIds.MediaType);
        public virtual MetadataField MixArtist => FieldsById.GetOrDefault(CommonFieldIds.MixArtist);
        public virtual MetadataField Mood => FieldsById.GetOrDefault(CommonFieldIds.Mood);
        public virtual MetadataField MovementName => FieldsById.GetOrDefault(CommonFieldIds.MovementName);
        public virtual MetadataField Movement => FieldsById.GetOrDefault(CommonFieldIds.Movement);
        public virtual MetadataField MovementTotal => FieldsById.GetOrDefault(CommonFieldIds.MovementTotal);
        public virtual MetadataField MusicianCredits => FieldsById.GetOrDefault(CommonFieldIds.MusicianCredits);
        public virtual MetadataField NetRadioOwner => FieldsById.GetOrDefault(CommonFieldIds.NetRadioOwner);
        public virtual MetadataField NetRadioStation => FieldsById.GetOrDefault(CommonFieldIds.NetRadioStation);
        public virtual MetadataField OrigAlbum => FieldsById.GetOrDefault(CommonFieldIds.OrigAlbum);
        public virtual MetadataField OrigArtist => FieldsById.GetOrDefault(CommonFieldIds.OrigArtist);
        public virtual MetadataField OrigFileName => FieldsById.GetOrDefault(CommonFieldIds.OrigFileName);
        public virtual MetadataField OrigLyricist => FieldsById.GetOrDefault(CommonFieldIds.OrigLyricist);
        public virtual MetadataField OrigReleaseTime => FieldsById.GetOrDefault(CommonFieldIds.OrigReleaseTime);
        public virtual MetadataField Publisher => FieldsById.GetOrDefault(CommonFieldIds.Publisher);
        public virtual MetadataField Rating_Mm => FieldsById.GetOrDefault(CommonFieldIds.Rating_Mm);
        public virtual MetadataField Rating_WinAmp => FieldsById.GetOrDefault(CommonFieldIds.Rating_WinAmp);
        public virtual MetadataField Rating_Wmp => FieldsById.GetOrDefault(CommonFieldIds.Rating_Wmp);
        public virtual MetadataField ReleaseTime => FieldsById.GetOrDefault(CommonFieldIds.ReleaseTime);
        public virtual MetadataField SetSubtitle => FieldsById.GetOrDefault(CommonFieldIds.SetSubtitle);
        public virtual MetadataField Subtitle => FieldsById.GetOrDefault(CommonFieldIds.Subtitle);
        public virtual MetadataField TaggingTime => FieldsById.GetOrDefault(CommonFieldIds.TaggingTime);
        public virtual MetadataField UnsyncedLyrics => FieldsById.GetOrDefault(CommonFieldIds.UnsyncedLyrics);
        public virtual MetadataField WwwArtist => FieldsById.GetOrDefault(CommonFieldIds.WwwArtist);
        public virtual MetadataField WwwAudioFile => FieldsById.GetOrDefault(CommonFieldIds.WwwAudioFile);
        public virtual MetadataField WwwAudioSource => FieldsById.GetOrDefault(CommonFieldIds.WwwAudioSource);
        public virtual MetadataField WwwCommercialInfo => FieldsById.GetOrDefault(CommonFieldIds.WwwCommercialInfo);
        public virtual MetadataField WwwCopyright => FieldsById.GetOrDefault(CommonFieldIds.WwwCopyright);
        public virtual MetadataField WwwPayment => FieldsById.GetOrDefault(CommonFieldIds.WwwPayment);
        public virtual MetadataField WwwPublisher => FieldsById.GetOrDefault(CommonFieldIds.WwwPublisher);
        public virtual MetadataField WwwRadioPage => FieldsById.GetOrDefault(CommonFieldIds.WwwRadioPage);
    }
}
