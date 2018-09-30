using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;
using TagLib.Ogg;
using TagLib.Riff;

namespace FuwaTea.Audio.Metadata.Impl.TagLib
{
    public static class TagLibUtils
    {
        public static IMetadata ReadFrom(Tag src)
        {
            if (src == null) return null;
            switch (src.TagTypes)
            {
                case TagTypes.None:
                    return null;
                case TagTypes.FlacMetadata:
                    return ReadFrom(src as global::TagLib.Flac.Metadata);
                case TagTypes.Xiph:
                    return ReadFrom(src as XiphComment);
                case TagTypes.Ape:
                    return ReadFrom(src as global::TagLib.Ape.Tag);
                case TagTypes.Id3v2:
                    return ReadFrom(src as global::TagLib.Id3v2.Tag);
                case TagTypes.Asf:
                    return ReadFrom(src as global::TagLib.Asf.Tag);
                case TagTypes.Apple:
                    return ReadFrom(src as global::TagLib.Mpeg4.AppleTag);
                case TagTypes.RiffInfo:
                    return ReadFrom(src as InfoTag);
                case TagTypes.Id3v1:
                    return ReadFrom(src as global::TagLib.Id3v1.Tag);
                default:
                    throw new NotSupportedException("Unsupported tag format");
            }
        }

        public static IMetadata ReadFrom(XiphComment src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(global::TagLib.Id3v1.Tag src)
        {
            if (src == null) return null;
            var tag = new Id3V11();
            tag.Title.Value = src.Title;
            tag.Artist.Value = src.Performers.ToList();
            tag.Album.Value = src.Album;
            tag.Year.Year = (ushort)src.Year;
            tag.Comment.Value = new List<string> { src.Comment };
            tag.Track.Value = src.Track;
            tag.Genre.Value = src.Genres.ToList();
            return tag;
        }

        public static IMetadata ReadFrom(global::TagLib.Id3v2.Tag src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(global::TagLib.Ape.Tag src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(global::TagLib.Mpeg4.AppleTag src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(global::TagLib.Asf.Tag src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(InfoTag src)
        {
            if (src == null) return null;
            // TODO: Implement
            return null;
        }

        public static IMetadata ReadFrom(global::TagLib.Flac.Metadata src)
        {
            if (src == null) return null;
            var x = src.GetComment(false, null);
            var t = ReadFrom(x);
            if (t == null)
            {
                if (src.Pictures.Length < 1) return null;
                t = new StringBasedMetadata();
            }

            foreach (var pic in src.Pictures)
                t.Picture.Add(pic.ToPicture());

            return t;
        }

        public static Picture ToPicture(this global::TagLib.IPicture pic)
        {
            return new Picture
            {
                MimeType = pic.MimeType,
                Description = pic.Description,
                PictureType = pic.Type,
                Data = pic.Data.Data
            };
        }
    }
}
