using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DryIocAttributes;
using FTWPlayer.Properties;
using FTWPlayer.ViewModels.SettingsViewModels;
using FTWPlayer.Views;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Settings Tab")]
    [Reuse(ReuseType.Singleton)]
    public class SettingsViewModel : ITab
    {
        
        public SettingsViewModel([ImportMany] IEnumerable<ISettingsTab> settingsTabs)
        {
            TabObject = new SettingsView(this);
            SettingsTabs = new ObservableCollection<TabItem>(settingsTabs.OrderBy(t => t.Index).Select(t => t.GetTabItem(Settings.Default, ((App)Application.Current).DynSettings)));
        }
        public TabItem TabObject { get; }
        public decimal Index => 3;
        public ObservableCollection<TabItem> SettingsTabs { get; }
    }
}
