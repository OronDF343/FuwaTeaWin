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
    [ContentProperty(nameof(ExcludedElements))]
    public class WindowDragBehavior : ControlledBehaviorBase<Window>
    {
        private static readonly DependencyPropertyKey ExcludedElementsPropertyKey =
            DependencyProperty.RegisterReadOnly("ExcludedElements", typeof(FreezableCollection<ElementReference>),
                                        typeof(WindowDragBehavior), new PropertyMetadata(new FreezableCollection<ElementReference>()));

        public static readonly DependencyProperty ExcludedElementsProperty =
            ExcludedElementsPropertyKey.DependencyProperty;

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
        private bool _dragConfirmed;

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ExcludedElements.Any(el => el.Binding != null && el.Binding.IsMouseOver)) return;
            _dragConfirmed = true;
            _startPoint = e.GetPosition(AssociatedObject);
            _previous = AssociatedObject.ResizeMode;
            AssociatedObject.ResizeMode = ResizeMode.NoResize;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragConfirmed) return;
            var newPoint = e.GetPosition(AssociatedObject);
            if (e.LeftButton == MouseButtonState.Pressed
                && Enabled
                && (Math.Abs(newPoint.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(newPoint.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                AssociatedObject.DragMove();
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_dragConfirmed) return;
            AssociatedObject.ResizeMode = _previous;
            _dragConfirmed = false;
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
