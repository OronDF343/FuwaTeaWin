using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Localization;
using FTWPlayer.Views;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Sage.Audio.Decoders;
using Sage.Audio.Files;
using Sage.Audio.Playback;
using Sage.Lib.Models;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Library Tab")]
    public class LibraryViewModel : ITab
    {
        private readonly IProtocolManager _pm;

        public LibraryViewModel([Import] IProtocolManager pm, [Import] ISubTrackEnumerationManager stem, [Import] IPlaybackManager playbackManager)
        {
            _pm = pm;
            TabObject = new LibraryView(this);
            PlaylistManager = stem;
            PlaybackManager = playbackManager;
            OpenPlaylistCommand = new RelayCommand<RoutedEventArgs>(OpenPlaylist);
            SavePlaylistCommand = new RelayCommand<RoutedEventArgs>(SavePlaylist);
            SaveAsCommand = new RelayCommand<RoutedEventArgs>(SaveAs);
        }
        public TabItem TabObject { get; }
        public decimal Index => 2;
        public ISubTrackEnumerationManager PlaylistManager { get; }
        public IPlaybackManager PlaybackManager { get; }

        private string OpenFileFilter => LocalizationProvider.GetLocalizedValue<string>("AllSupportedPlaylists")
                                         + "|*" + string.Join(";*.", PlaylistManager.SupportedFormats);
        // TODO: Playlist saving
        private string SaveFileFilter => string.Join("|", from w in PlaylistManager.SupportedFormats
                                                          let x = "*." + w
                                                          select $"{string.Format(LocalizationProvider.GetLocalizedValue<string>("FileTypeFormatString"), w.ToUpperInvariant(), x)}|{x}");

        public ICommand OpenPlaylistCommand { get; set; }

        private OpenFileDialog _ofd;
        private void OpenPlaylist(RoutedEventArgs e)
        {
            if (_ofd == null)
                _ofd = new OpenFileDialog { Filter = OpenFileFilter, Title = "Open Playlist" };
            if (_ofd.ShowDialog(Application.Current.MainWindow) != true) return;
            var h = _pm.Handle(new FileLocationInfo(new Uri(_ofd.FileName)));
            var pr = PlaylistManager.FirstCanHandleOrDefault(h);
            PlaybackManager.List = new ObservableCollection<IFileHandle>(pr.Handle(h).Select(t => pr.Handle(t)));
        }

        public ICommand SavePlaylistCommand { get; set; }

        private void SavePlaylist(RoutedEventArgs e)
        {
            // TODO: Save playlist
            /*if (!File.Exists(PlaybackManager.SelectedPlaylist?.FileLocation))
                SaveAs(e);
            else PlaybackManager.Save(PlaybackManager.SelectedPlaylist);*/
        }

        public ICommand SaveAsCommand { get; set; }

        private SaveFileDialog _sfd;
        private void SaveAs(RoutedEventArgs e)
        {
            
            if (_sfd == null)
                _sfd = new SaveFileDialog { Filter = SaveFileFilter, Title = "Save Playlist As..." };
            // TODO: Save playlist
            /*if (_sfd.ShowDialog(Application.Current.MainWindow) != true) return;
            PlaybackManager.SaveTo(PlaybackManager.SelectedPlaylist, _sfd.FileName);*/
        }
    }
}
