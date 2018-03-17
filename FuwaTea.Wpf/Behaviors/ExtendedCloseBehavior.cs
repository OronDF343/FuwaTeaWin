#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using log4net;

namespace FuwaTea.Wpf.Behaviors
{
    public class ExtendedCloseBehavior : AnimationBehaviorBase<Window>
    {
        public static readonly DependencyProperty VolumeFadeOutProperty =
            DependencyProperty.Register("VolumeFadeOut", typeof(bool), typeof(ExtendedCloseBehavior), new PropertyMetadata(true));

        public bool VolumeFadeOut
        {
            get => (bool)GetValue(VolumeFadeOutProperty);
            set => SetValue(VolumeFadeOutProperty, value);
        }

        public static readonly DependencyProperty VolumeFadeOutStoryboardProperty =
           DependencyProperty.Register("VolumeFadeOutStoryboard", typeof(Storyboard), typeof(ExtendedCloseBehavior), new PropertyMetadata(default(Storyboard)));

        public Storyboard VolumeFadeOutStoryboard
        {
            get => (Storyboard)GetValue(VolumeFadeOutStoryboardProperty);
            set => SetValue(VolumeFadeOutStoryboardProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            LogManager.GetLogger(GetType()).Debug("Detected window closing");
            if (Storyboard == null || !Enabled) return;
            LogManager.GetLogger(GetType()).Debug("Animation is enabled");
            e.Cancel = true;
            AssociatedObject.Closing -= OnWindowClosing;

            // VolumeFadeOut logic
            if (VolumeFadeOut)
            {
                LogManager.GetLogger(GetType()).Debug("Volume fade out is enabled");
                Storyboard.Children.Add(VolumeFadeOutStoryboard);
            }

            Storyboard.Completed += (o, a) =>
            {
                LogManager.GetLogger(GetType()).Debug("Animation finished - Closing window");
                AssociatedObject.Close();
            };
            Storyboard.Begin(AssociatedObject);
        }
    }
}
