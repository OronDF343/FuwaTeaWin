using System.Windows;
using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    public class UserTabItem : TabItem
    {
        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(DependencyObject), typeof(UserTabItem));

        public new DependencyObject Parent { get { return (DependencyObject)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
    }
}
