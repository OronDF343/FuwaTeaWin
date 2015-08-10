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
using System.Linq;
using System.Runtime.CompilerServices;
using FuwaTea.Annotations;
using FuwaTea.Common.Models;
using FuwaTea.Data.Playlist;
using LayerFramework;

namespace FuwaTea.Logic.Playlist
{
    [LogicElement("Playlist Manager")]
    public class PlaylistManager : IPlaylistManager
    {
        public PlaylistManager()
        {
            LoadedPlaylists = new Dictionary<string, IPlaylist>();
        }

        public Dictionary<string, IPlaylist> LoadedPlaylists { get; }

        private string _selectedId;
        public string SelectedPlaylistId
        {
            get { return _selectedId; }
            set
            {
                if (LoadedPlaylists.ContainsKey(value)) _selectedId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        public IPlaylist SelectedPlaylist => SelectedPlaylistId != null && LoadedPlaylists.ContainsKey(SelectedPlaylistId) ? LoadedPlaylists[SelectedPlaylistId] : null;
        public IEnumerable<string> ReadableFileTypes => LayerFactory.GetElements<IPlaylistReader>().SelectMany(r => r.SupportedFileTypes);

        public void CreatePlaylist(string name)
        {
            LoadedPlaylists.Add(name, new Playlist());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
