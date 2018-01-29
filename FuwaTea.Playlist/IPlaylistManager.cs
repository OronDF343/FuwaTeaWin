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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using JetBrains.Annotations;

namespace FuwaTea.Playlist
{
    [InheritedExport]
    public interface IPlaylistManager : IPlaylistHandler, INotifyPropertyChanged
    {
        [NotNull]
        Dictionary<string, IPlaylist> LoadedPlaylists { get; }
        string SelectedPlaylistId { get; set; }
        [CanBeNull]
        IPlaylist SelectedPlaylist { get; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> ReadableFileTypes { get; }
        IEnumerable<string> WriteableFileTypes { get; }

        void CreatePlaylist(string name);
        void MergePlaylists(IPlaylist source, IPlaylist target);


        IPlaylist OpenPlaylist(string path);
        void Save(IPlaylist playlist);
        void SaveTo(IPlaylist playlist, string path);
        void SaveCopy(IPlaylist playlist, string path);
    }
}
