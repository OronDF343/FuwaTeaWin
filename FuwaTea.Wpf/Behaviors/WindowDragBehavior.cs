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

using System.Windows;
using System.Windows.Input;

namespace FuwaTea.Wpf.Behaviors
{
    public class WindowDragBehavior : ControlledBehaviorBase<Window>
    {
        public static readonly DependencyProperty ExcludedControlProperty =
            DependencyProperty.Register("ExcludedControl", typeof(FrameworkElement), typeof(WindowDragBehavior), new PropertyMetadata(null));

        public FrameworkElement ExcludedControl // TODO: Make this a collection somehow
        {
            get { return (FrameworkElement)GetValue(ExcludedControlProperty); }
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
