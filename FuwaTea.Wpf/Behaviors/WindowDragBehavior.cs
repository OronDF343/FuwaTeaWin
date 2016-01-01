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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace FuwaTea.Wpf.Behaviors
{
    [ContentProperty(nameof(ExcludedElements))]
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

        public static readonly DependencyProperty EnableNegativePositionProperty =
            DependencyProperty.Register("EnableNegativePosition", typeof(bool), typeof(WindowDragBehavior),
                                        new PropertyMetadata(true));

        public bool EnableNegativePosition
        {
            get { return (bool)GetValue(EnableNegativePositionProperty); }
            set { SetValue(EnableNegativePositionProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AddSourceHook();
            AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
            base.OnAttached();
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            AddSourceHook();
        }

        private void AddSourceHook()
        {
            if (_hwnd != null) return;
            _hwnd = (HwndSource)PresentationSource.FromVisual(AssociatedObject);
            _hwnd?.AddHook(HwndSourceHook); // should never be null
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
            _hwnd?.RemoveHook(HwndSourceHook);
            _hwnd = null;
            base.OnDetaching();
        }

        private HwndSource _hwnd;
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
            if (e.LeftButton == MouseButtonState.Pressed && Enabled
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

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        };

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (hwnd == _hwnd.Handle && msg == 0x46 && EnableNegativePosition && _dragConfirmed && Mouse.LeftButton != MouseButtonState.Pressed) // WM_WINDOWPOSCHANGING
            {
                var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                wp.flags = wp.flags | 2; // SWP_NOMOVE
                Marshal.StructureToPtr(wp, lParam, false);
            }
            return IntPtr.Zero;
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
