using System.Windows.Controls;

namespace FTWPlayer.ViewModels
{
    [UIPart("Help Tab")]
    public class HelpViewModel : ITab
    {
        public HelpViewModel() { TabObject = new TabItem { Header = "HELP" }; }
        public TabItem TabObject { get; }
        public decimal Index => 4;
    }
}
