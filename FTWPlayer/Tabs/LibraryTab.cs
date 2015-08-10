using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    [UIPart("Library Tab")]
    public class LibraryTab : ITab
    {
        public LibraryTab() { TabObject = new TabItem {Header = "LIBRARY"}; }
        public TabItem TabObject { get; }
        public decimal Index => 2;
    }
}
