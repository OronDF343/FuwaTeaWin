using System.Windows;
using System.Windows.Interactivity;

namespace FuwaTea.Wpf.Behaviors
{
    public class ControlledBehaviorBase<T> : Behavior<T>
        where T : FrameworkElement
    {
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(ControlledBehaviorBase<T>), new PropertyMetadata(true));

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }
    }
}
