using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace FuwaTea.Wpf.Behaviors
{
    public class ExtendedCloseBehavior : AnimationBehaviorBase<Window>
    {
        public static readonly DependencyProperty VolumeFadeOutProperty =
            DependencyProperty.Register("VolumeFadeOut", typeof(bool), typeof(ExtendedCloseBehavior), new PropertyMetadata(true));

        public bool VolumeFadeOut
        {
            get { return (bool)GetValue(VolumeFadeOutProperty); }
            set { SetValue(VolumeFadeOutProperty, value); }
        }

        public static readonly DependencyProperty VolumeFadeOutStoryboardProperty =
           DependencyProperty.Register("VolumeFadeOutStoryboard", typeof(Storyboard), typeof(ExtendedCloseBehavior), new PropertyMetadata(default(Storyboard)));

        public Storyboard VolumeFadeOutStoryboard
        {
            get { return (Storyboard)GetValue(VolumeFadeOutStoryboardProperty); }
            set { SetValue(VolumeFadeOutStoryboardProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (Storyboard == null || !Enabled) return;
            e.Cancel = true;
            AssociatedObject.Closing -= OnWindowClosing;

            // VolumeFadeOut logic
            if (VolumeFadeOut)
                Storyboard.Children.Add(VolumeFadeOutStoryboard);

            Storyboard.Completed += (o, a) => AssociatedObject.Close();
            Storyboard.Begin(AssociatedObject);
        }
    }
}
