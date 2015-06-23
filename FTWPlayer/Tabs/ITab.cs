using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    public interface ITab : IUIPart
    {
        TabItem TabObject { get; }
        decimal Index { get; }
    }
}
