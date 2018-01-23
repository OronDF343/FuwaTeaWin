using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    public interface ISettingsTab : IUIPart
    {
        TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings);
        decimal Index { get; }
    }
}
