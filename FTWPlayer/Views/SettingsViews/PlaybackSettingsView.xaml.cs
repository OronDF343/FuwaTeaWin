using FTWPlayer.ViewModels.SettingsViewModels;

namespace FTWPlayer.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for PlaybackSettingsView.xaml
    /// </summary>
    public partial class PlaybackSettingsView
    {
        public PlaybackSettingsView(PlaybackSettingsViewModel psvm)
        {
            DataContext = psvm;
            InitializeComponent();
        }
    }
}
