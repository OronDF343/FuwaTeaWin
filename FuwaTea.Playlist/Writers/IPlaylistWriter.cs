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

using System.ComponentModel.Composition;
using FuwaTea.Lib;

namespace FuwaTea.Playlist.Writers
{
    [InheritedExport]
    public interface IPlaylistWriter : IFileHandler, IPlaylistHandler
    {
        /// <summary>
        /// Writes a list of entries to a playlist file.
        /// </summary>
        /// <param name="path">The path to the playlist file.</param>
        /// <param name="playlist">The playlist which will be written to the file.</param>
        /// <param name="relativePaths"></param>
        void WritePlaylist(string path, IPlaylist playlist, bool relativePaths);
    }
}
