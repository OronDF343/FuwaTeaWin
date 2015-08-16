using System.Windows.Controls;
using FTWPlayer.Views;
using FuwaTea.Common.Models;
using FuwaTea.Logic.Playlist;
using LayerFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Library Tab")]
    public class LibraryViewModel : ITab
    {
        public LibraryViewModel()
        {
            TabObject = new LibraryView(this);
            PlaylistManager = LayerFactory.GetElement<IPlaylistManager>(); // TODO: remove logic reference
        }
        public TabItem TabObject { get; }
        public decimal Index => 2;
        public IPlaylistManager PlaylistManager { get; }
        public IPlaylist Playlist { get; private set; }
    }
}
