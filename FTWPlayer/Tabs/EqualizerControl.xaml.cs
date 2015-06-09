using System.Windows.Controls;
using FTWPlayer.ViewModel;

namespace FTWPlayer.Tabs
{
    /// <summary>
    /// Interaction logic for EqualizerControl.xaml
    /// </summary>
    public partial class EqualizerControl : UserControl
    {
        public EqualizerControl(MainViewModel mvm)
        {
            InitializeComponent();
            DataContext = mvm;
        }
    }
}
