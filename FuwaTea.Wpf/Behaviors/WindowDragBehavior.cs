using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FuwaTea.Wpf.Behaviors
{
    public class WindowDragBehavior : ControlledBehaviorBase<Window>
    {
        public static readonly DependencyProperty ExcludedControlProperty =
            DependencyProperty.Register("ExcludedControl", typeof(Control), typeof(WindowDragBehavior), new PropertyMetadata(null));

        public Control ExcludedControl // TODO: Make this a collection somehow
        {
            get { return (Control)GetValue(ExcludedControlProperty); }
            set { SetValue(ExcludedControlProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += OnMouseDown;
            base.OnAttached();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left && Enabled
                && (ExcludedControl == null || !ExcludedControl.IsMouseOver)) AssociatedObject.DragMove(); // TODO: All tabs must have backgroud set so this works.
        }
    }
}
