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
