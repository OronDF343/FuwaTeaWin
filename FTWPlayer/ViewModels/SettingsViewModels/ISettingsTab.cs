using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;
using FuwaTea.Extensibility.ConfigurationTemp;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    public interface ISettingsTab : IUIPart
    {
        TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings);
        decimal Index { get; }
    }
}
