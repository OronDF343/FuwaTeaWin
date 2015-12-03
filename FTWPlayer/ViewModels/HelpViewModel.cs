using System.Windows.Controls;
using System.Windows.Data;
using WPFLocalizeExtension.Extensions;

namespace FTWPlayer.ViewModels
{
    [UIPart("Help Tab")]
    public class HelpViewModel : ITab
    {
        public HelpViewModel()
        {
            TabObject = new TabItem();
            BindingOperations.SetBinding(TabObject, HeaderedContentControl.HeaderProperty, new BLoc("HelpTabHeader"));
        }
        public TabItem TabObject { get; }
        public decimal Index => 4;
    }
}
