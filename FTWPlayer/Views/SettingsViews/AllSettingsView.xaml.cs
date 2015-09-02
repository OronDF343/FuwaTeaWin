using FTWPlayer.ViewModels.SettingsViewModels;

namespace FTWPlayer.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for AllSettingsView.xaml
    /// </summary>
    public partial class AllSettingsView
    {
        public AllSettingsView(AllSettingsViewModel asvm)
        {
            DataContext = asvm;
            InitializeComponent();
        }
    }
}
