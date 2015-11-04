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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace FuwaTea.Wpf.Behaviors
{
    [ContentProperty("ExcludedElements")]
    public class WindowDragBehavior : ControlledBehaviorBase<Window>
    {
        public WindowDragBehavior()
        {
            SetValue(ExcludedElementsProperty, new FreezableCollection<ElementReference>());
        }

        public static readonly DependencyProperty ExcludedElementsProperty =
            DependencyProperty.Register("ExcludedElements", typeof(FreezableCollection<ElementReference>),
                                        typeof(WindowDragBehavior));

        public FreezableCollection<ElementReference> ExcludedElements
            => (FreezableCollection<ElementReference>)GetValue(ExcludedElementsProperty);

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            base.OnAttached();
        }

        private Point _startPoint;
        private ResizeMode _previous;

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(AssociatedObject);
            _previous = AssociatedObject.ResizeMode;
            AssociatedObject.ResizeMode = ResizeMode.NoResize;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var newPoint = e.GetPosition(AssociatedObject);
            if (e.LeftButton == MouseButtonState.Pressed && Enabled
                && !ExcludedElements.Any(el => el.Binding != null && el.Binding.IsMouseOver)
                && (Math.Abs(newPoint.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(newPoint.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                AssociatedObject.DragMove();
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ResizeMode = _previous;
        }
    }

    public class ElementReference : Freezable
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding",
                                                                                                typeof(UIElement),
                                                                                                typeof(ElementReference));

        public UIElement Binding
        {
            get { return (UIElement)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ElementReference();
        }
    }
}
