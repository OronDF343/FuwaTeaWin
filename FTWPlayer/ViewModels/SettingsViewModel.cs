using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using FTWPlayer.ViewModels.SettingsViewModels;
using FTWPlayer.Views;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Settings Tab")]
    public class SettingsViewModel : ITab
    {
        
        public SettingsViewModel([ImportMany] IEnumerable<ISettingsTab> settingsTabs, [Import] IConfigManager configManager)
        {
            // TODO IMPORTANT: Dynamically create missing settings page tabs!!!
            TabObject = new SettingsView(this);
            SettingsTabs = new ObservableCollection<TabItem>(settingsTabs.OrderBy(t => t.Index).Select(t => t.GetTabItem()));
        }
        public TabItem TabObject { get; }
        public decimal Index => 3;
        public ObservableCollection<TabItem> SettingsTabs { get; }
    }
}
