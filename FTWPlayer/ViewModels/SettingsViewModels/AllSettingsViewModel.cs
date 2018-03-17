using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Controls;
using DryIocAttributes;
using FTWPlayer.Views.SettingsViews;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("All Settings Tab")]
    [Reuse(ReuseType.Singleton)]
    public class AllSettingsViewModel : ISettingsTab
    {
        public TabItem GetTabItem()
        {
            // TODO: Remove this tab because it is broken
            //SettingsPropertyValues = new ObservableCollection<SettingsPropertyValue>(settings.PropertyValues.OfType<SettingsPropertyValue>().OrderBy(spv => spv.Name));
            return new AllSettingsView(this);
        }

        public decimal Index => 1;
        public ObservableCollection<SettingsPropertyValue> SettingsPropertyValues { get; private set; }
    }
}
