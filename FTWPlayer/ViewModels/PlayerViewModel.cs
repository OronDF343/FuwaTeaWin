using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FTWPlayer.Views;
using FuwaTea.Metadata;
using FuwaTea.Metadata.AlbumArt;
using FuwaTea.Playback;
using ModularFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Player Tab")]
    public class PlayerViewModel : ITab, INotifyPropertyChanged
    {
        public PlayerViewModel()
        {
            PlaybackManager = ModuleFactory.GetElement<IPlaybackManager>();
            TabObject = new AlbumArtView(this);
            PlaybackManager.PropertyChanged += PlaybackManager_PropertyChanged;
        }

        private void PlaybackManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: Still leaks memory, also memory is not freed when changing to null. At least changing to a smaller image helps *somewhat*.
            // For now, users who care about the app using over 100MB of memory should avoid loading very large images or switching between songs too often.
            if (e.PropertyName != nameof(PlaybackManager.Current)) return;
            if (PlaybackManager.Current == null)
            {
                CurrentAlbumArt = null;
                return;
            }
            var s = ModuleFactory.GetElement<IAlbumArtLocator>().GetAlbumArt(PlaybackManager.Current);
            if (s == null)
            {
                CurrentAlbumArt = null;
                return;
            }
            using (s)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = s;
                bi.EndInit();
                CurrentAlbumArt = bi;
                s.Close();
            }
        }

        private ImageSource _albumArt;
        [UsedImplicitly]
        public ImageSource CurrentAlbumArt
        {
            get { return _albumArt; }
            private set
            {
                _albumArt = value;
                OnPropertyChanged();
            }
        }

        public TabItem TabObject { get; }
        public decimal Index => 0;

        public IPlaybackManager PlaybackManager { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
