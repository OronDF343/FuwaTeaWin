

using FTWPlayer.ViewModels.SettingsViewModels;

namespace FTWPlayer.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for KeyBindingsView.xaml
    /// </summary>
    public partial class KeyBindingsView
    {
        public KeyBindingsView(KeyBindingsViewModel kbvm)
        {
            DataContext = kbvm;
            InitializeComponent();
        }
    }
}
