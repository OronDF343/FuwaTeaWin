using System;
using System.Linq;
using System.Text;
using TagLib;
using TagLib.Mpeg4;
using TagLib.Ogg;
using TagLib.Riff;

namespace FuwaTea.Audio.Metadata.Utils
{
    /*public static class TagLibUtils
    {
        public static bool ReadFrom(this IMetadata meta, Tag src)
        {
            if (src == null) return false;
            switch (src.TagTypes)
            {
                case TagTypes.None:
                    return false;
                case TagTypes.FlacMetadata:
                    return ReadFrom(meta, src as TagLib.Flac.Metadata);
                case TagTypes.Xiph:
                    return ReadFrom(meta, src as XiphComment);
                case TagTypes.Ape:
                    return ReadFrom(meta, src as TagLib.Ape.Tag);
                case TagTypes.Id3v2:
                    return ReadFrom(meta, src as TagLib.Id3v2.Tag);
                case TagTypes.Apple:
                    return ReadFrom(meta, src as AppleTag);
                case TagTypes.Asf:
                    return ReadFrom(meta, src as TagLib.Asf.Tag);
                case TagTypes.RiffInfo:
                    return ReadFrom(meta, src as InfoTag);
                case TagTypes.Id3v1:
                    return ReadFrom(meta, src as TagLib.Id3v1.Tag);
                default:
                    throw new NotSupportedException("Unsupported tag format");
            }
        }

        public static bool ReadFrom(this IMetadata meta, XiphComment src)
        {
            if (src == null) return false;
            foreach (var key in src)
                if (!meta.ContainsKey(key))
                    meta.Add(key, new TextField(src.GetField(key).ToList()));
            return true;
        }

        public static bool ReadFrom(this IMetadata meta, TagLib.Id3v1.Tag src)
        {
            if (src == null) return false;
            if (!meta.ContainsKey(nameof(src.Title)))
                meta.Add(nameof(src.Title), new TextField(src.Title, maxLen:30, encoding:Encoding.ASCII));
            if (!meta.ContainsKey(nameof(src.Performers)))
                meta.Add(nameof(src.Performers), new TextField(src.Performers, maxLen: 30, encoding: Encoding.ASCII));
            if (!meta.ContainsKey(nameof(src.Album)))
                meta.Add(nameof(src.Album), new TextField(src.Album, maxLen: 30, encoding: Encoding.ASCII));
            if (!meta.ContainsKey(nameof(src.Year)))
                meta.Add(nameof(src.Year), new NumericField(value: src.Year, bits: 14));
            if (!meta.ContainsKey(nameof(src.Comment)))
                meta.Add(nameof(src.Comment), new TextField(src.Comment, maxLen: 30, encoding: Encoding.ASCII));
            if (!meta.ContainsKey(nameof(src.Genres)))
                meta.Add(nameof(src.Genres), new EnumField<StandardGenre>((StandardGenre)Genres.AudioToIndex(src.Genres.FirstOrDefault())));
            if (!meta.ContainsKey(nameof(src.Track)))
                meta.Add(nameof(src.Track), new NumericField(value: src.Track, bits: 8));
            return true;
        }

        public static bool ReadFrom(this IMetadata meta, TagLib.Id3v2.Tag src)
        {
            if (src == null) return false;
            return false;
        }

        public static bool ReadFrom(this IMetadata meta, TagLib.Ape.Tag src)
        {
            if (src == null) return false;
            return false;
        }

        public static bool ReadFrom(this IMetadata meta, AppleTag src)
        {
            if (src == null) return false;
            return false;
        }

        public static bool ReadFrom(this IMetadata meta, TagLib.Asf.Tag src)
        {
            if (src == null) return false;
            return false;
        }

        public static bool ReadFrom(this IMetadata meta, InfoTag src)
        {
            if (src == null) return false;
            return false;
        }

        public static bool ReadFrom(this IMetadata meta, TagLib.Flac.Metadata src)
        {
            if (src == null) return false;
            foreach (var picture in src.Pictures.GroupBy(p => p.Type))
            {
                if (!meta.ContainsKey(picture.Key))
                    meta.Add(picture.Key, new PictureField(picture.ToList()));
            }
            var x = src.GetComment(false, null);
            return ReadFrom(meta, x) || src.Pictures.Length > 0;
        }
    }*/
}
