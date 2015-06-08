using System;
using System.IO;
using TagLib;

namespace FuwaTea.Data.Playlist
{
    public class MusicInfoModel
    {
        public Guid UniqueId { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public string FilePath { get { return FileInfo.FullName; } }
        public string FileName { get { return FileInfo.Name; } }
        public string FileType { get { return Path.GetExtension(FilePath); } }
        public TimeSpan Duration { get; private set; }
        public int Bitrate { get; private set; }
        public Tag Tag { get; private set; }

        public MusicInfoModel(string path, Tag tag, TimeSpan duration, int bitrate)
        {
            FileInfo = new FileInfo(path);
            Tag = tag;
            Duration = duration;
            Bitrate = bitrate;
            UniqueId = Guid.NewGuid();
        }
    }
}
