using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    [UIPart("Help Tab")]
    public class HelpTab : ITab
    {
        public HelpTab() { TabObject = new TabItem { Header = "HELP" }; }
        public TabItem TabObject { get; private set; }
        public decimal Index { get { return 4; } }
    }
}
