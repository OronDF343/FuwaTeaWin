using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FTWPlayer.ViewModels
{
    [InheritedExport]
    public interface ITab : IUIPart
    {
        TabItem TabObject { get; }
        decimal Index { get; }
    }
}
