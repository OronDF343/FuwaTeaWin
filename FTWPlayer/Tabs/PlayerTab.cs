using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FuwaTea.Annotations;
using FuwaTea.Logic.Playlist.AlbumArt;
using FuwaTea.Presentation.Playback;
using LayerFramework;

namespace FTWPlayer.Tabs
{
    [UIPart("Player Tab")]
    public class PlayerTab : ITab
    {
        public PlayerTab()
        {
            PlaybackManager = LayerFactory.GetElement<IPlaybackManager>();
            TabObject = new AlbumArtDisplay(this);
        }

        #region Testing: Album Art Display

        [UsedImplicitly]
        public ImageSource CurrentAlbumArt
        {
            get
            {
                if (PlaybackManager.Current == null) return null;
                var s = LayerFactory.GetElement<IAlbumArtLocator>().GetAlbumArt(PlaybackManager.Current);
                if (s == null) return null;
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = s;
                bi.EndInit();
                return bi;
                // TODO: CLEAN UP MEMORY USAGE - STILL IN USE AFTER SWITCHING - HIGH USAGE
            }
        }

        #endregion

        public TabItem TabObject { get; private set; }
        public decimal Index { get { return 0; } }

        public IPlaybackManager PlaybackManager { get; private set; }
    }
}
