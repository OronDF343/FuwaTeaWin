using System.Collections.Generic;
using System.Linq;
using Sage.Lib;
using Serilog;
using TagLib;
using TagLib.Id3v2;
using TagLib.Ogg;
using TagLib.Riff;
using File = TagLib.File;
using Tag = TagLib.Tag;

namespace Sage.Audio.Metadata.Impl.TagLib
{
    public static class TagLibUtils
    {
        public static IMetadata ReadFrom(File f)
        {
            IMetadata m = null;
            if (f.TagTypes.HasFlags(TagTypes.FlacMetadata)) m = ReadFrom(f.GetTag(TagTypes.FlacMetadata) as global::TagLib.Flac.Metadata);
            else if (f.TagTypes.HasFlags(TagTypes.Xiph)) m = ReadFrom(f.GetTag(TagTypes.Xiph) as XiphComment);
            else if (f.TagTypes.HasFlags(TagTypes.Ape)) m = ReadFrom(f.GetTag(TagTypes.Ape) as global::TagLib.Ape.Tag);
            else if (f.TagTypes.HasFlags(TagTypes.Id3v2)) m = ReadFrom(f.GetTag(TagTypes.Id3v2) as global::TagLib.Id3v2.Tag);
            else if (f.TagTypes.HasFlags(TagTypes.Asf)) m = ReadFrom(f.GetTag(TagTypes.Asf) as global::TagLib.Asf.Tag);
            else if (f.TagTypes.HasFlags(TagTypes.Apple)) m = ReadFrom(f.GetTag(TagTypes.Apple) as global::TagLib.Mpeg4.AppleTag);
            else if (f.TagTypes.HasFlags(TagTypes.RiffInfo)) m = ReadFrom(f.GetTag(TagTypes.RiffInfo) as InfoTag);
            else if (f.TagTypes.HasFlags(TagTypes.Id3v1)) m = ReadFrom(f.GetTag(TagTypes.Id3v1) as global::TagLib.Id3v1.Tag);
            return m;
        }

        public static IMetadata ReadFrom(Tag src)
        {
            if (src == null) return null;
            switch (src.TagTypes)
            {
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
                    return null;
            }
        }

        // TODO: WriteTo

        public static IMetadata ReadFrom(XiphComment src)
        {
            if (src == null) return null;
            var tag = new StringBasedMetadata();
            var cFields = new HashSet<string>(CommonFieldIds.All);
            foreach (var key in src.Select(k => k.ToUpperInvariant()))
                if (cFields.Contains(key)) tag.Fields[key].SetFrom(src.GetField(key));
                else tag.AddCustomField(key).SetFrom(src.GetField(key));
            return tag;
        }

        public static IMetadata ReadFrom(global::TagLib.Id3v1.Tag src)
        {
            if (src == null) return null;
            var tag = new Id3V11();
            tag.Title.SetFrom(src.Title);
            tag.Artist.SetFrom(src.Performers);
            tag.Album.SetFrom(src.Album);
            tag.Year.SetFrom(src.Year);
            tag.Comment.SetFrom(src.Comment);
            tag.Track.SetFrom(src.Track);
            tag.Genre.SetFrom(src.Genres);
            return tag;
        }

        public static IMetadata ReadFrom(global::TagLib.Id3v2.Tag src)
        {
            if (src == null) return null;
            // IMPORTANT: TagLib converts frame ids of ID3v2.2/3 to ID3v2.4. I'll exploit this to support ID3v2.2 by treating it like ID3v2.3.
            // * TODO: ID3v2.2 tags should always be upgraded on writing.
            // * IMPORTANT: This means that ID3v2.3 should also be treated as ID3v2.4 in compatibility mode
            var tag = new Id3V24(compatV3: src.Version < 4);
            foreach (var frame in src.GetFrames())
            {
                // BUG: If a TXXX or WXXX frame exists with a description equal to an existing frame ID, a collision may occur. For now, these frames will be ignored if necessary.
                switch (frame)
                {
                    // BUG: TagLib does not comply with ID3v2.3/4 spec for user frames - It reads multiple strings from a single frame as a null-separated list
                    // http://id3.org/id3v2.3.0 | http://id3.org/id3v2.4.0-frames
                    // Section 4.2.2 (v2.3) | 4.2.6 (v2.4): This frame is intended for one-string text information
                    // Action taken: Only first string is taken (since custom fields are always BasicTextFields)
                    case UserTextInformationFrame utif when !tag.Fields.ContainsKey(utif.Description):
                        tag.AddCustomField(utif.Description).SetFrom(utif.Text);
                        break;
                    case UserTextInformationFrame utif:
                        Log.Warning($"[ID3v2.x] Skipping user text information frame (TXXX) with description \"{utif.Description}\" due to a conflict with an existing tag");
                        break;
                    // BUG: TagLib does not comply with ID3v2.3 spec for text information frames - It reads multiple strings from a single frame as a null-separated list
                    // v2.3 Section 4.2: If the textstring is followed by a termination ($00 (00)) all the following information should be ignored and not be displayed
                    // v2.4 allows this (Section 4.2)
                    // Action taken: For v2.3 and for custom mappings: Only the first string is taken per frame.
                    //               For v2.4: All are taken, but only when the underlying field type supports multiple values.
                    //                         TODO: This may cause issues for jonied lists.
                    case TextInformationFrame tif:
                        if (tif.Text.Length < 1) break;
                        switch (tif.FrameId.ToString())
                        {
                            case "TDAT":
                                var dtf = (IDateTimeField)tag.Year;
                                dtf.Day = byte.Parse(tif.Text[0].Substring(0, 2));
                                dtf.Month = byte.Parse(tif.Text[0].Substring(2, 2));
                                break;
                            case "TIME":
                                var dtf2 = (IDateTimeField)tag.Year;
                                dtf2.Hour = byte.Parse(tif.Text[0].Substring(0, 2));
                                dtf2.Minute = byte.Parse(tif.Text[0].Substring(2, 2));
                                break;
                            case "TRCK":
                                var t = tif.Text[0].Split('/');
                                tag.Track.SetFrom(t[0]);
                                if (t.Length > 1) tag.TrackCount.SetFrom(t[1]);
                                break;
                            case "TPOS":
                                var d = tif.Text[0].Split('/');
                                tag.Disc.SetFrom(d[0]);
                                if (d.Length > 1) tag.DiscCount.SetFrom(d[1]);
                                break;
                            default:
                                if (src.Version < 4)
                                    tag.Fields[CommonFieldIds.Id3V2ToFieldId[tif.FrameId.ToString()]].SetFrom(tif.Text.FirstOrDefault());
                                else tag.Fields[CommonFieldIds.Id3V2ToFieldId[tif.FrameId.ToString()]].SetFrom(tif.Text);
                                break;
                        }
                        break;
                    case UnsynchronisedLyricsFrame ulf:
                        ((IListWithDescriptorsField)tag.UnsyncedLyrics).Value.Add(new EntryWithDescriptors(ulf.Language, ulf.Description, ulf.Text));
                        break;
                    case CommentsFrame cf:
                        ((IListWithDescriptorsField)tag.Comment).Value.Add(new EntryWithDescriptors(cf.Language, cf.Description, cf.Text));
                        break;
                    case AttachedPictureFrame apf:
                        tag.Picture.Add(apf.ToPicture());
                        break;
                    // TODO: POPM has a user field
                    case PopularimeterFrame pf:
                        tag.Rating_Mm.SetFrom(pf.Rating);
                        tag.Rating_WinAmp.SetFrom(pf.Rating);
                        tag.Rating_Wmp.SetFrom(pf.Rating);
                        break;
                    // BUG: Our version of TagLib does not parse WXXX frames correctly (it treats them like any other URL frame)
                    // * Source code lines 154-259 at https://github.com/PeterHagen/taglib-sharp/blob/master/src/TagLib/Id3v2/FrameFactory.cs
                    case UserUrlLinkFrame uurlf when !tag.Fields.ContainsKey(uurlf.Description):
                        tag.AddCustomField(uurlf.Description).SetFrom(uurlf.Text);
                        break;
                    case UserUrlLinkFrame uurlf:
                        Log.Warning($"[ID3v2.x] Skipping user URL link frame (wXXX) with description \"{uurlf.Description}\" due to a conflict with an existing tag");
                        break;
                    // BUG: TagLib does not comply with ID3v2.4 spec for URL frames - It reads multiple URLs from a single frame as a null-separated list
                    // Section 4.3 of the spec: "If the text string is followed by a string termination, all the following information should be ignored and not be displayed."
                    // Action taken: Only first string is taken per frame
                    case UrlLinkFrame urlf:
                        tag.Fields[CommonFieldIds.Id3V2ToFieldId[urlf.FrameId.ToString()]].SetFrom(urlf.Text.FirstOrDefault());
                        break;
                }
                // TODO: TagLib does not support the following frames at all: IPLS, GRP1, MVNM, MVIN. Consider adding a workaround.
            }
            return tag;
        }

        public static IMetadata ReadFrom(global::TagLib.Ape.Tag src)
        {
            if (src == null) return null;
            var tag = new StringBasedMetadata();
            var cFields = new HashSet<string>(CommonFieldIds.All);
            foreach (var key in src.Select(k => k.ToUpperInvariant()))
                if (cFields.Contains(key)) tag.Fields[key].SetFrom(src.GetItem(key).ToStringArray());
                else tag.AddCustomField(key).SetFrom(src.GetItem(key).ToStringArray());
            return tag;
        }

        public static IMetadata ReadFrom(global::TagLib.Mpeg4.AppleTag src)
        {
            if (src == null) return null;
            var tag = new AppleTag();
            foreach (var f in CommonFieldIds.AppleToFieldId)
                tag.Fields[f.Value].SetFrom(src.GetText(f.Key));
            return tag;
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
            var tag = new RiffInfo();
            tag.Title.SetFrom(src.Title);
            tag.Artist.SetFrom(src.Performers);
            tag.AlbumArtist.SetFrom(src.AlbumArtists);
            tag.Year.SetFrom(src.Year);
            tag.Track.SetFrom(src.Track);
            tag.TrackCount.SetFrom(src.TrackCount);
            tag.Genre.SetFrom(src.Genres);
            tag.Comment.SetFrom(src.Comment);
            tag.Composer.SetFrom(src.Composers);
            tag.Copyright.SetFrom(src.Copyright);
            // TODO: Extra tags by ID
            return tag;
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
