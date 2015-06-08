using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FuwaTea.Data.Playlist
{
    [DataElement("M3U / M3U8 playlist reader")]
    public class M3UPlaylistReader : IPlaylistReader
    {
        public IEnumerable<string> GetPlaylistFiles(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path).TrimEnd('\\');
                return new List<string>(from f in File.ReadAllLines(path)
                                        where !string.IsNullOrWhiteSpace(f) && !f.StartsWith("#EXT")
                                        select f[1] == ':' || f.StartsWith("\\\\") ? f : (dir + "\\" + f));
            }
            catch (Exception e)
            {
                throw new DataSourceException(path, "Failed to read M3U playlist!", e);
            }
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] {".m3u", ".m3u8"}; } }
    }
}
