using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FuwaTea.Lib;

namespace FuwaTea.Data.Playlist
{
    [DataElement("M3U / M3U8 playlist writer")]
    public class M3UPlaylistWriter : IPlaylistWriter
    {
        public void WritePlaylist(string path, IEnumerable<string> entries, bool relativePaths)
        {
            try
            {
                File.WriteAllLines(path, entries.Select(s => relativePaths ? PathUtils.MakeRelativePath(path, s) : s), path.EndsWith("8") ? Encoding.UTF8 : Encoding.Default);
            }
            catch (Exception e)
            {
                throw new DataSourceException(path, "Failed to write M3U8 playlist!", e);
            }
        }

        public IEnumerable<string> SupportedFileTypes { get { return new[] { ".m3u", ".m3u8" }; } } // TODO: add description for each type
    }
}
