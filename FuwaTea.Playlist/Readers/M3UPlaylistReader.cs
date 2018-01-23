#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using FuwaTea.Lib;
using FuwaTea.Lib.Exceptions;

namespace FuwaTea.Playlist.Readers
{
    //[PlaylistHandler("M3U / M3U8 playlist reader")]
    [Export(typeof(IPlaylistReader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class M3UPlaylistReader : IPlaylistReader
    {
        public IEnumerable<string> SupportedFileTypes => new[] {"m3u|M3U Playlist", "m3u8|M3U Playlist (UTF-8)"};
        public void LoadPlaylistFiles(string path, IPlaylist playlist)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                playlist.Init(path, from f in File.ReadAllLines(path, path.EndsWith("8") ? Encoding.UTF8 : Encoding.GetEncoding(1252))
                                    where !string.IsNullOrWhiteSpace(f) && !f.StartsWith("#EXT")
                                    select PathUtils.ExpandRelativePath(dir, f));
            }
            catch (Exception e)
            {
                throw new DataSourceException(path, "Failed to read M3U playlist!", e);
            }
        }
    }
}
