using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using FTWPlayer.Properties;
using FTWPlayer.ViewModels.SettingsViewModels;
using FTWPlayer.Views;
using ModularFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Settings Tab")]
    public class SettingsViewModel : ITab
    {
        public SettingsViewModel()
        {
            TabObject = new SettingsView(this);
            SettingsTabs = new ObservableCollection<TabItem>(ModuleFactory.GetElements<ISettingsTab>().OrderBy(t => t.Index).Select(t => t.GetTabItem(Settings.Default)));
        }
        public TabItem TabObject { get; }
        public decimal Index => 3;
        public ObservableCollection<TabItem> SettingsTabs { get; }
    }
}
