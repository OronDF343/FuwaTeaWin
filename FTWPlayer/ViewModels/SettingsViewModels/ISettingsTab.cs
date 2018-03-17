using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [InheritedExport]
    public interface ISettingsTab : IUIPart
    {
        TabItem GetTabItem();
        decimal Index { get; }
    }
}
