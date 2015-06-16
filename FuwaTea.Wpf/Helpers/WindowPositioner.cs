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
