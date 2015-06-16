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
