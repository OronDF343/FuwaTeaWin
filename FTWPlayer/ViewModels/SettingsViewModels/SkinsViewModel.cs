using System.Configuration;
using System.Windows.Controls;
using FTWPlayer.Views.SettingsViews;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [UIPart("Skin Selection")]
    public class SkinsViewModel : ISettingsTab
    {
        public TabItem GetTabItem(ApplicationSettingsBase settings)
        {
            return new SkinsView(this);
        }

        public decimal Index => 2;
    }
}
