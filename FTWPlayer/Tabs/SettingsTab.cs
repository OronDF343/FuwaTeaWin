using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    [UIPart("Settings Tab")]
    public class SettingsTab : ITab
    {
        public SettingsTab()
        {
            TabObject = new SettingsView();
        }
        public TabItem TabObject { get; private set; }
        public decimal Index { get { return 3; } }
    }
}
