using System.Collections.Generic;
using System.ComponentModel;
using FuwaTea.Annotations;
using FuwaTea.Common.Models;

namespace FuwaTea.Logic.Playlist
{
    public interface IPlaylistManager : ILogicElement, INotifyPropertyChanged
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

        void CreatePlaylist(string name);
    }
}
