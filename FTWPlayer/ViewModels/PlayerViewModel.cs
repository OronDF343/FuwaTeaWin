using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DryIocAttributes;
using FTWPlayer.Views;
using FuwaTea.Audio.Playback;
using JetBrains.Annotations;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Player Tab")]
    [Reuse(ReuseType.Singleton)]
    public class PlayerViewModel : ITab, INotifyPropertyChanged
    {
        
        public PlayerViewModel([Import] IPlaybackManager playbackManager/*, [Import] IAlbumArtLocator albumArtLocator*/)
        {
            PlaybackManager = playbackManager;
            //AlbumArtLocator = albumArtLocator;
            TabObject = new AlbumArtView(this);
            PlaybackManager.PropertyChanged += PlaybackManager_PropertyChanged;
        }

        private void PlaybackManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Users who care about the app using over 100MB of memory should avoid loading very large images or switching between songs too often.
            if (e.PropertyName != nameof(PlaybackManager.Index)) return;
            if (PlaybackManager.NowPlaying == null)
            {
                CurrentAlbumArt = null;
                return;
            }
            // TODO AlbumArt
            /*var s = AlbumArtLocator.GetAlbumArt(PlaybackManager.NowPlaying);
            if (s == null)
            {
                CurrentAlbumArt = null;
                return;
            }
            using (s)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = s;
                bi.EndInit();
                bi.Freeze();
                CurrentAlbumArt = bi;
                s.Close();
            }*/
        }

        private ImageSource _albumArt;
        [UsedImplicitly]
        public ImageSource CurrentAlbumArt
        {
            get => _albumArt;
            private set
            {
                _albumArt = value;
                OnPropertyChanged();
            }
        }

        public TabItem TabObject { get; }
        public decimal Index => 0;

        public IPlaybackManager PlaybackManager { get; }
        //public IAlbumArtLocator AlbumArtLocator { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
