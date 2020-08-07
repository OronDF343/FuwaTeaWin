using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using PropertyChanged;

namespace Sage.Helpers
{
    [DoNotNotify]
    public class WindowDragBehavior : Behavior<Window>
    {
        public static readonly AvaloniaProperty<ObservableCollection<ElementReference>> ExcludedElementsProperty =
            AvaloniaProperty
                .Register<WindowDragBehavior, ObservableCollection<ElementReference>>(nameof(ExcludedElements),
                                                                                      new ObservableCollection<
                                                                                          ElementReference>());

        public ObservableCollection<ElementReference> ExcludedElements
        {
            get => GetValue(ExcludedElementsProperty);
            set => SetValue(ExcludedElementsProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.PointerPressed += PointerPressed;
            //AssociatedObject.PointerMoved += PointerMoved;
            //AssociatedObject.PointerReleased += PointerReleased;
            base.OnAttached();
        }

        //public double MinimumVerticalDragDistance { get; set; } = 10.0;

        //public double MinimumHorizontalDragDistance { get; set; } = 10.0;

        //private Point _startPoint;
        //private ResizeMode _previous;
        //private bool _dragConfirmed;
        //private PointerPressedEventArgs _args;

        private void PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (ExcludedElements.Any(el => el.Binding != null && el.Binding.IsPointerOver))
            {
                //_dragConfirmed = false;
                return;
            }

            AssociatedObject.BeginMoveDrag(e);
            //_dragConfirmed = true;
            //_startPoint = e.GetPosition(AssociatedObject);
            //_args = e;
            //_previous = AssociatedObject.ResizeMode;
            //AssociatedObject.ResizeMode = ResizeMode.NoResize;
        }

        /*private void PointerMoved(object sender, PointerEventArgs e)
        {
            if (!_dragConfirmed) return;
            var newPoint = e.GetPosition(AssociatedObject);
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed //&& Enabled
                && (Math.Abs(newPoint.X - _startPoint.X) > MinimumHorizontalDragDistance
                    || Math.Abs(newPoint.Y - _startPoint.Y) > MinimumVerticalDragDistance))
            {
                AssociatedObject.BeginMoveDrag(new PointerPressedEventArgs(e.Source, e.Pointer, AssociatedObject, e.GetCurrentPoint(null).Position, e.Timestamp, e.GetCurrentPoint(null).Properties, e.KeyModifiers, 1));
            }
        }

        private void PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (!_dragConfirmed) return;
            //AssociatedObject.ResizeMode = _previous;
            _dragConfirmed = false;
            _args = null;
        }*/
    }

    [DoNotNotify]
    public class ElementReference : AvaloniaObject
    {
        public static readonly AvaloniaProperty<InputElement> BindingProperty =
            AvaloniaProperty.Register<ElementReference, InputElement>(nameof(Binding));

        public InputElement Binding { get => GetValue(BindingProperty); set => SetValue(BindingProperty, value); }
    }
}
