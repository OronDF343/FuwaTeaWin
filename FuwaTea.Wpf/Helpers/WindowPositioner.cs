using System.Globalization;
using System.Windows;

namespace FuwaTea.Wpf.Helpers
{
    public class WindowPositioner
    {
        public static readonly DependencyProperty AutoPositionProperty =
            DependencyProperty.RegisterAttached("AutoPosition",
                                                typeof(WindowPositions),
                                                typeof(WindowPositioner),
                                                new UIPropertyMetadata(AutoPositionChanged));

        public static WindowPositions GetAutoPosition(DependencyObject obj)
        {
            return (WindowPositions)obj.GetValue(AutoPositionProperty);
        }

        public static void SetAutoPosition(DependencyObject obj, WindowPositions value)
        {
            obj.SetValue(AutoPositionProperty, value);
        }

        private static void AutoPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window)) return;
            var window = (Window)d;
            var pos = (WindowPositions)e.NewValue;
            var desktopWorkingArea = SystemParameters.WorkArea;
            var isRtl = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
            switch (pos)
            {
                case WindowPositions.BottomRight:
                    window.Left = isRtl ? 0 : desktopWorkingArea.Right - window.Width;
                    window.Top = desktopWorkingArea.Bottom - window.Height;
                    break;
                case WindowPositions.BottomLeft:
                    window.Left = isRtl ? desktopWorkingArea.Right - window.Width : 0;
                    window.Top = desktopWorkingArea.Bottom - window.Height;
                    break;
                case WindowPositions.TopRight:
                    window.Left = isRtl ? 0 : desktopWorkingArea.Right - window.Width;
                    window.Top = 0;
                    break;
                case WindowPositions.TopLeft:
                    window.Left = isRtl ? desktopWorkingArea.Right - window.Width : 0;
                    window.Top = 0;
                    break;
            }
        }
    }

    public enum WindowPositions
    {
        Null,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
