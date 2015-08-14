using System.Windows.Controls;
using FTWPlayer.Views;

namespace FTWPlayer.ViewModels
{
    [UIPart("Library Tab")]
    public class LibraryViewModel : ITab
    {
        public LibraryViewModel()
        {
            TabObject = new LibraryView(this);
        }
        public TabItem TabObject { get; }
        public decimal Index => 2;
    }
}
