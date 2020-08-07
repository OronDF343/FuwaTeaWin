using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using PropertyChanged;

namespace Sage.Helpers
{
    [DoNotNotify]
    public class OverlayOnKeyBehavior : Behavior<Control>
    {
        public static readonly AvaloniaProperty<InputElement> FocusRootProperty =
            AvaloniaProperty.Register<ElementReference, InputElement>(nameof(FocusRoot));

        public InputElement FocusRoot { get => GetValue(FocusRootProperty); set => SetValue(FocusRootProperty, value); }

        protected override void OnAttached()
        {
            base.OnAttached();
            FocusRoot.KeyDown += FocusRoot_KeyDown;
            FocusRoot.KeyUp += FocusRoot_KeyUp;
            AssociatedObject.ZIndex = -1;
        }

        private void FocusRoot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                AssociatedObject.ZIndex = 10;
        }

        private void FocusRoot_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                AssociatedObject.ZIndex = -10;
        }

        protected override void OnDetaching()
        {
            FocusRoot.KeyDown -= FocusRoot_KeyDown;
            FocusRoot.KeyUp -= FocusRoot_KeyUp;
            base.OnDetaching();
        }
    }
}
