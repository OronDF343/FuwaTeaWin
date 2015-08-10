using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using FTWPlayer.Properties;

namespace FTWPlayer.Tabs
{
    [UIPart("Settings Tab")]
    public class SettingsTab : ITab
    {
        public SettingsTab()
        {
            TabObject = new SettingsView(this);
            SettingsPropertyValues = new ObservableCollection<SettingsPropertyValue>(Settings.Default.PropertyValues.OfType<SettingsPropertyValue>());
        }
        public TabItem TabObject { get; }
        public decimal Index => 3;
        public ObservableCollection<SettingsPropertyValue> SettingsPropertyValues { get; private set; } 
    }
}
