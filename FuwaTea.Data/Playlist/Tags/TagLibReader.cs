using System.Collections.Generic;
using System.IO;
using TagLib;

namespace FuwaTea.Data.Playlist.Tags
{
    [DataElement("TagLib# tag reader")]
    public class TagLibReader : ITagReader
    {
        public MusicInfoModel ReadTag(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tagFile = TagLib.File.Create(new StreamFileAbstraction(path, stream, stream));
                var model = new MusicInfoModel(path, tagFile.Tag, tagFile.Properties.Duration,
                                               tagFile.Properties.AudioBitrate);
                tagFile.Dispose(); // TODO: check if there is ObjectDisposedException
                stream.Close();
                return model;
            }
        }

        public IEnumerable<string> SupportedFileTypes 
        {
            get // TODO: double-check
            {
                return new[]
                {
                    ".aac",
                    ".m4a",
                    ".aiff",
                    ".ape",
                    ".asf",
                    ".aud",
                    ".flac",
                    ".ogg",
                    ".mp3",
                    ".wav",
                    ".wv",
                    ".adts",
                    ".wma",
                    ".mka"
                };
            }
        }
    }
}
