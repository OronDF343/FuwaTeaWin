using System;
using System.Windows;
using System.Windows.Controls;

namespace FuwaTea.Wpf.Helpers
{
    // Credit: Thomas Danemar https://tdanemar.wordpress.com/2009/11/15/using-the-visualstatemanager-with-the-model-view-viewmodel-pattern-in-wpf-or-silverlight/
    public class StateManager : DependencyObject
    {
        public static string GetVisualState(DependencyObject obj)
        {
            return (string)obj.GetValue(VisualStateProperty);
        }

        public static void SetVisualState(DependencyObject obj, string value)
        {
            obj.SetValue(VisualStateProperty, value);
        }

        public static readonly DependencyProperty VisualStateProperty =
            DependencyProperty.RegisterAttached("VisualState",
                                                typeof(string),
                                                typeof(StateManager),
                                                new PropertyMetadata(OnVisualStateChanged));

        private static void OnVisualStateChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = s as Control;
            if (ctrl == null)
                throw new InvalidOperationException("This attached property only supports types derived from Control.");
            VisualStateManager.GoToState(ctrl, (string)e.NewValue, true);
        }
    }
}
