using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FuwaTea.Common.Models
{
    // Derived from code taken from TagLib.Portable
    public abstract class TagData
    {
        //protected Tag();

        public static string Separator { get; set; } = ", ";

        public virtual string Album { get; set; }
        public virtual string[] AlbumArtists { get; set; }
        public virtual string[] AlbumArtistsSort { get; set; }
        //public virtual string AlbumSort { get; set; }
        //public virtual string AmazonId { get; set; }
        public virtual uint BeatsPerMinute { get; set; }
        public virtual string Comment { get; set; }
        public virtual string[] Composers { get; set; }
        public virtual string[] ComposersSort { get; set; }
        public virtual string Conductor { get; set; }
        public virtual string Copyright { get; set; }
        public virtual uint Disc { get; set; }
        public virtual uint DiscCount { get; set; }
        public string FirstAlbumArtist => AlbumArtists.FirstOrDefault();
        public string FirstAlbumArtistSort => AlbumArtistsSort.FirstOrDefault();
        public string FirstComposer => Composers.FirstOrDefault();
        public string FirstComposerSort => ComposersSort.FirstOrDefault();
        public string FirstGenre => Genres.First();
        public string FirstPerformer => Performers.First();
        public string FirstPerformerSort => PerformersSort.First();
        public virtual string[] Genres { get; set; }
        public virtual string Grouping { get; set; }
        public abstract bool IsEmpty { get; } // HasTag
        public string JoinedAlbumArtists => string.Join(Separator, AlbumArtists);
        public string JoinedAlbumArtistsSort => string.Join(Separator, AlbumArtistsSort);
        public string JoinedComposers => string.Join(Separator, Composers);
        public string JoinedComposersSort => string.Join(Separator, ComposersSort);
        public string JoinedGenres => string.Join(Separator, Genres);
        public string JoinedPerformers => string.Join(Separator, Performers);
        public string JoinedPerformersSort => string.Join(Separator, PerformersSort);
        public virtual string Lyrics { get; set; }
        //public virtual string MusicBrainzArtistId { get; set; }
        //public virtual string MusicBrainzDiscId { get; set; }
        //public virtual string MusicBrainzReleaseArtistId { get; set; }
        //public virtual string MusicBrainzReleaseCountry { get; set; }
        //public virtual string MusicBrainzReleaseId { get; set; }
        //public virtual string MusicBrainzReleaseStatus { get; set; }
        //public virtual string MusicBrainzReleaseType { get; set; }
        //public virtual string MusicBrainzTrackId { get; set; }
        //public virtual string MusicIpId { get; set; }
        public virtual string[] Performers { get; set; }
        public virtual string[] PerformersSort { get; set; }
        public virtual IPicture[] Pictures { get; set; }
        //public virtual double ReplayGainAlbumGain { get; set; }
        //public virtual double ReplayGainAlbumPeak { get; set; }
        //public virtual double ReplayGainTrackGain { get; set; }
        //public virtual double ReplayGainTrackPeak { get; set; }
        public virtual TagTypes TagTypes { get; }
        public virtual string Title { get; set; }
        //public virtual string TitleSort { get; set; }
        public virtual uint Track { get; set; }
        public virtual uint TrackCount { get; set; }
        public virtual uint Year { get; set; }
        
        public abstract void Clear();
        //public virtual void CopyTo(TagData target, bool overwrite);
    }

    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TagTypes : uint
    {
        None = 0,
        Xiph = 1,
        Id3v1 = 2,
        Id3v2 = 4,
        Ape = 8,
        Apple = 16,
        Asf = 32,
        RiffInfo = 64,
        MovieId = 128,
        DivX = 256,
        FlacMetadata = 512,
        TiffIFD = 1024,
        AudibleMetadata = 1024,
        XMP = 2048,
        JpegComment = 4096,
        GifComment = 8192,
        Png = 16384,
        IPTCIIM = 32768,
        AllTags = uint.MaxValue
    }

    public interface IPicture
    {
        byte[] Data { get; }
        string Description { get; }
        string MimeType { get; }
        PictureType Type { get; }
    }

    public enum PictureType
    {
        Other = 0,
        FileIcon = 1,
        OtherFileIcon = 2,
        FrontCover = 3,
        BackCover = 4,
        LeafletPage = 5,
        Media = 6,
        LeadArtist = 7,
        Artist = 8,
        Conductor = 9,
        Band = 10,
        Composer = 11,
        Lyricist = 12,
        RecordingLocation = 13,
        DuringRecording = 14,
        DuringPerformance = 15,
        MovieScreenCapture = 16,
        ColoredFish = 17,
        Illustration = 18,
        BandLogo = 19,
        PublisherLogo = 20
    }
}
