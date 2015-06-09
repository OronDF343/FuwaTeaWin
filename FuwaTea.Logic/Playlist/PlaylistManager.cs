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

        public Dictionary<string, IPlaylist> LoadedPlaylists { get; private set; }

        private string _selectedId;
        public string SelectedPlaylistId
        {
            get { return _selectedId; }
            set
            {
                if (LoadedPlaylists.ContainsKey(value)) _selectedId = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedPlaylist");
            }
        }

        public IPlaylist SelectedPlaylist { get { return SelectedPlaylistId != null && LoadedPlaylists.ContainsKey(SelectedPlaylistId) ? LoadedPlaylists[SelectedPlaylistId] : null; } }
        public IEnumerable<string> ReadableFileTypes { get { return LayerFactory.GetElements<IPlaylistReader>().SelectMany(r => r.SupportedFileTypes); } }

        public void CreatePlaylist(string name)
        {
            LoadedPlaylists.Add(name, new Playlist());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
