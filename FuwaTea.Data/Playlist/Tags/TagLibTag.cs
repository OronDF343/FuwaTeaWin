using System;
using System.IO;
using System.Linq;
using log4net;
using TagLib;
using File = TagLib.File;
using IPicture = FuwaTea.Common.Models.IPicture;
using PictureType = FuwaTea.Common.Models.PictureType;
using Tag = FuwaTea.Common.Models.Tag;
using TagTypes = FuwaTea.Common.Models.TagTypes;

namespace FuwaTea.Data.Playlist.Tags
{
    public class TagLibTag : Tag
    {
        private readonly TagLib.Tag _tag;
        private readonly string _path;
        public TagLibTag(string path)
        {
            _path = path;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                File tagFile = null;
                try { tagFile = File.Create(new StreamFileAbstraction(path, stream, stream)); }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Error("Failed to load tag!", e);
                }
                Bitrate = tagFile?.Properties.AudioBitrate ?? 0;
                Duration = tagFile?.Properties.Duration ?? TimeSpan.Zero;
                _tag = tagFile?.Tag ?? new TagLib.Id3v2.Tag(); // TODO: Create correct tag type
                tagFile?.Dispose();
                stream.Close();
            }
        }

        public override void Clear()
        {
            _tag.Clear();
        }

        public override bool IsWriteSupported => true;

        public override void SaveTags()
        {
            using (var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                File tagFile = null;
                try { tagFile = File.Create(new StreamFileAbstraction(_path, stream, stream)); }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Error("Failed to load tag!", e);
                }
                if (tagFile != null) _tag.CopyTo(tagFile.Tag, true);
                tagFile?.Save();
                tagFile?.Dispose();
                stream.Close();
            }
        }
        
        public override string Album => _tag.Album;
        public override string[] AlbumArtists => _tag.AlbumArtists;
        public override string[] AlbumArtistsSort => _tag.AlbumArtistsSort;
        //public override string AlbumSort { get; set; }
        //public override string AmazonId { get; set; }
        public override uint BeatsPerMinute => _tag.BeatsPerMinute;
        public override string Comment => _tag.Comment;
        public override string[] Composers => _tag.Composers;
        public override string[] ComposersSort => _tag.ComposersSort;
        public override string Conductor => _tag.Conductor;
        public override string Copyright => _tag.Copyright;
        public override uint Disc => _tag.Disc;
        public override uint DiscCount => _tag.DiscCount;
        public override string[] Genres => _tag.Genres;
        public override string Grouping => _tag.Grouping;
        public override bool IsEmpty => _tag.IsEmpty; // HasTag
        public override string Lyrics => _tag.Lyrics;
        //public override string MusicBrainzArtistId { get; set; }
        //public override string MusicBrainzDiscId { get; set; }
        //public override string MusicBrainzReleaseArtistId { get; set; }
        //public override string MusicBrainzReleaseCountry { get; set; }
        //public override string MusicBrainzReleaseId { get; set; }
        //public override string MusicBrainzReleaseStatus { get; set; }
        //public override string MusicBrainzReleaseType { get; set; }
        //public override string MusicBrainzTrackId { get; set; }
        //public override string MusicIpId { get; set; }
        public override string[] Performers => _tag.Performers;
        public override string[] PerformersSort => _tag.PerformersSort;
        public override IPicture[] Pictures
            => _tag.Pictures.Select(p => new Picture(p.Data.Data, p.Description, p.MimeType, (PictureType)(int)p.Type))
                   .Cast<IPicture>()
                   .ToArray();
        //public override double ReplayGainAlbumGain { get; set; }
        //public override double ReplayGainAlbumPeak { get; set; }
        //public override double ReplayGainTrackGain { get; set; }
        //public override double ReplayGainTrackPeak { get; set; }
        public override TagTypes TagTypes => (TagTypes)(uint)_tag.TagTypes;
        public override string Title => _tag.Title;
        //public virtual string TitleSort { get; set; }
        public override uint Track => _tag.Track;
        public override uint TrackCount => _tag.TrackCount;
        public override uint Year => _tag.Year;
    }
}
