using System.Windows.Controls;

namespace FTWPlayer.ViewModels
{
    public interface ITab : IUIPart
    {
        TabItem TabObject { get; }
        decimal Index { get; }
    }
}
