using System.Collections.Generic;
using System.Windows;

namespace FuwaTea.Wpf.Helpers
{
    public class FlowDirectionUpdater
    {
        public static readonly DependencyProperty AutoUpdateModeProperty =
            DependencyProperty.RegisterAttached("AutoUpdateMode",
                                                typeof(AutoUpdateMode),
                                                typeof(FlowDirectionUpdater),
                                                new UIPropertyMetadata(AutoUpdateMode.Disabled, AutoUpdateModeChanged));

        public static AutoUpdateMode GetAutoUpdateMode(DependencyObject obj)
        {
            return (AutoUpdateMode)obj.GetValue(AutoUpdateModeProperty);
        }

        public static void SetAutoUpdateMode(DependencyObject obj, AutoUpdateMode value)
        {
            obj.SetValue(AutoUpdateModeProperty, value);
        }

        private static readonly List<FrameworkElement> AttachedElements = new List<FrameworkElement>();

        private static void AutoUpdateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((AutoUpdateMode)e.OldValue == AutoUpdateMode.Disabled && (AutoUpdateMode)e.NewValue != AutoUpdateMode.Disabled) AttachedElements.Add((FrameworkElement)d);
            if ((AutoUpdateMode)e.OldValue != AutoUpdateMode.Disabled && (AutoUpdateMode)e.NewValue == AutoUpdateMode.Disabled) AttachedElements.Remove((FrameworkElement)d);
        }

        public static void UpdateFlowDirection(FlowDirection fd)
        {
            foreach (var element in AttachedElements)
            {
                switch (GetAutoUpdateMode(element))
                {
                    case AutoUpdateMode.Enabled:
                        element.FlowDirection = fd;
                        break;
                    case AutoUpdateMode.Reverse:
                        element.FlowDirection = fd == FlowDirection.LeftToRight
                                                    ? FlowDirection.RightToLeft
                                                    : FlowDirection.LeftToRight;
                        break;
                }
            }
        }
    }

    public enum AutoUpdateMode
    {
        Disabled,
        Enabled,
        Reverse
    }
}
