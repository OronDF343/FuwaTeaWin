using System.Windows;

namespace FuwaTea.Wpf.Behaviors
{
    public class OpenBehavior : AnimationBehaviorBase<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Storyboard == null || !Enabled) return;
            Storyboard.Begin(AssociatedObject);
        }
    }
}
