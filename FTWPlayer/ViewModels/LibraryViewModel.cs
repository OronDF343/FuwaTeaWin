using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Localization;
using FTWPlayer.Views;
using FuwaTea.Lib;
using FuwaTea.Playlist;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Library Tab")]
    [Export(typeof(ITab))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LibraryViewModel : ITab
    {
        [ImportingConstructor]
        public LibraryViewModel([Import] IPlaylistManager playlistManager)
        {
            TabObject = new LibraryView(this);
            PlaylistManager = playlistManager;
            OpenPlaylistCommand = new RelayCommand<RoutedEventArgs>(OpenPlaylist);
            SavePlaylistCommand = new RelayCommand<RoutedEventArgs>(SavePlaylist);
            SaveAsCommand = new RelayCommand<RoutedEventArgs>(SaveAs);
        }
        public TabItem TabObject { get; }
        public decimal Index => 2;
        public IPlaylistManager PlaylistManager { get; }

        private string OpenFileFilter => LocalizationProvider.GetLocalizedValue<string>("AllSupportedPlaylists")
                                         + "|*" + string.Join(";*", StringUtils.GetExtensions(PlaylistManager.ReadableFileTypes));
        // TODO: group them?
        private string SaveFileFilter => string.Join("|", from w in StringUtils.GetExtensionsInfo(PlaylistManager.WriteableFileTypes)
                                                          let x = "*." + w.Key
                                                          select $"{string.Format(LocalizationProvider.GetLocalizedValue<string>("FileTypeFormatString"), w.Value, x)}|{x}");

        public ICommand OpenPlaylistCommand { get; set; }

        private OpenFileDialog _ofd;
        private void OpenPlaylist(RoutedEventArgs e)
        {
            if (_ofd == null)
                _ofd = new OpenFileDialog { Filter = OpenFileFilter, Title = "Open Playlist" };
            if (_ofd.ShowDialog(Application.Current.MainWindow) != true) return;
            if (!PlaylistManager.LoadedPlaylists.ContainsKey(_ofd.FileName))
                PlaylistManager.LoadedPlaylists.Add(_ofd.FileName, PlaylistManager.OpenPlaylist(_ofd.FileName));
            PlaylistManager.SelectedPlaylistId = _ofd.FileName;
        }

        public ICommand SavePlaylistCommand { get; set; }

        private void SavePlaylist(RoutedEventArgs e)
        {
            if (!File.Exists(PlaylistManager.SelectedPlaylist?.FileLocation))
                SaveAs(e);
            else PlaylistManager.Save(PlaylistManager.SelectedPlaylist);
        }

        public ICommand SaveAsCommand { get; set; }

        private SaveFileDialog _sfd;
        private void SaveAs(RoutedEventArgs e)
        {
            if (_sfd == null)
                _sfd = new SaveFileDialog { Filter = SaveFileFilter, Title = "Save Playlist As..." };
            if (_sfd.ShowDialog(Application.Current.MainWindow) != true) return;
            // TODO: ID of playlist should change?
            PlaylistManager.SaveTo(PlaylistManager.SelectedPlaylist, _sfd.FileName);
        }
    }
}
