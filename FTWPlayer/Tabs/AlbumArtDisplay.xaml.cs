using System.Windows.Controls;
using FTWPlayer.ViewModel;

namespace FTWPlayer.Tabs
{
    /// <summary>
    /// Interaction logic for AlbumArtDisplay.xaml
    /// </summary>
    public partial class AlbumArtDisplay : UserControl
    {
        public AlbumArtDisplay(MainViewModel mv)
        {
            InitializeComponent();
            DataContext = mv;
        }
    }
}
