using FTWPlayer.ViewModels.SettingsViewModels;

namespace FTWPlayer.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for SkinsView.xaml
    /// </summary>
    public partial class SkinsView
    {
        public SkinsView(SkinsViewModel svm)
        {
            DataContext = svm;
            InitializeComponent();
        }
    }
}
