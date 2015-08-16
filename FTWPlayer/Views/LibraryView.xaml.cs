using FTWPlayer.ViewModels;

namespace FTWPlayer.Views
{
    /// <summary>
    /// Interaction logic for LibraryView.xaml
    /// </summary>
    public partial class LibraryView
    {
        public LibraryView(LibraryViewModel lt)
        {
            InitializeComponent();
            DataContext = lt;
        }
    }
}
