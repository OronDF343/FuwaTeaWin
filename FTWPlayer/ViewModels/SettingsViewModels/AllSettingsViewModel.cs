using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using FTWPlayer.Views.SettingsViews;
using ModularFramework.Configuration;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [UIPart("All Settings Tab")]
    public class AllSettingsViewModel : ISettingsTab
    {
        public TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings)
        {
            SettingsPropertyValues = new ObservableCollection<SettingsPropertyValue>(settings.PropertyValues.OfType<SettingsPropertyValue>().OrderBy(spv => spv.Name));
            return new AllSettingsView(this);
        }

        public decimal Index => 1;
        public ObservableCollection<SettingsPropertyValue> SettingsPropertyValues { get; private set; }
    }
}
