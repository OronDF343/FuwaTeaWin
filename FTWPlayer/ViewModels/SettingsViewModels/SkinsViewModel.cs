using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;
using FTWPlayer.Views.SettingsViews;
using ModularFramework.Configuration;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [UIPart("Skin Selection")]
    public class SkinsViewModel : ISettingsTab
    {
        public TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings)
        {
            return new SkinsView(this);
        }

        public decimal Index => 2;
    }
}
