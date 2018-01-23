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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DryIocAttributes;
using FuwaTea.Lib;
using FuwaTea.Metadata.Tags;
using FuwaTea.Playlist.Readers;
using FuwaTea.Playlist.Writers;
using JetBrains.Annotations;

namespace FuwaTea.Playlist
{
    //[PlaylistHandler("Playlist Manager")]
    [Export(typeof(IPlaylistManager))]
    [Reuse(ReuseType.Singleton)]
    public class PlaylistManager : IPlaylistManager
    {
        [ImportingConstructor]
        public PlaylistManager([ImportMany] IEnumerable<IPlaylistReader> readers,
                               [ImportMany] IEnumerable<IPlaylistWriter> writers,
                               [ImportMany] IEnumerable<ITagProvider> tagProviders)
        {
            _readers = readers.ToList();
            ReadableFileTypes = _readers.SelectMany(r => r.SupportedFileTypes).Distinct();
            _writers = writers.ToList();
            WriteableFileTypes = _writers.SelectMany(r => r.SupportedFileTypes).Distinct();
            _tagProviders = tagProviders.ToList();
        }

        public Dictionary<string, IPlaylist> LoadedPlaylists { get; } = new Dictionary<string, IPlaylist>();

        private string _selectedId;
        private readonly List<IPlaylistReader> _readers;
        private readonly List<IPlaylistWriter> _writers;
        private readonly List<ITagProvider> _tagProviders;

        public string SelectedPlaylistId
        {
            get => _selectedId;
            set
            {
                if (LoadedPlaylists.ContainsKey(value)) _selectedId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        public IPlaylist SelectedPlaylist => SelectedPlaylistId != null && LoadedPlaylists.ContainsKey(SelectedPlaylistId) ? LoadedPlaylists[SelectedPlaylistId] : null;
        public IEnumerable<string> ReadableFileTypes { get; }
        public IEnumerable<string> WriteableFileTypes { get; }

        public void CreatePlaylist(string name)
        {
            LoadedPlaylists.Add(name, new Playlist(_tagProviders));
        }

        public void MergePlaylists(IPlaylist source, IPlaylist target)
        {
            target.AddRange(source);
        }

        public IPlaylist OpenPlaylist(string path)
        {
            var pl = new Playlist(_tagProviders) {FileLocation = path};
            _readers.First(w => w.GetExtensions().Contains(Path.GetExtension(path).ToLowerInvariant())).LoadPlaylistFiles(path, pl);
            return pl;
        }

        public void Save(IPlaylist playlist)
        {
            if (string.IsNullOrEmpty(playlist.FileLocation)) throw new InvalidOperationException("No file open!");
            SaveTo(playlist, playlist.FileLocation);
        }

        public void SaveTo(IPlaylist playlist, string path)
        {
            SaveCopy(playlist, playlist.FileLocation = path);
            playlist.UnsavedChanges = false;
        }

        public void SaveCopy(IPlaylist playlist, string path)
        {
            _writers.First(w => w.GetExtensions().Contains(Path.GetExtension(path).ToLowerInvariant())).WritePlaylist(path, playlist, true); // TODO: place for relative path option etc
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
