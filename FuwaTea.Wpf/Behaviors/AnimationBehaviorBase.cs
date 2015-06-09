using System.Windows;
using System.Windows.Media.Animation;

namespace FuwaTea.Wpf.Behaviors
{
    public abstract class AnimationBehaviorBase<T> : ControlledBehaviorBase<T>
         where T : FrameworkElement
    {
        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(AnimationBehaviorBase<T>), new PropertyMetadata(default(Storyboard)));

        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }
    }
}
