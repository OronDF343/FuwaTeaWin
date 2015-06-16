﻿#region License
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FuwaTea.Wpf.Helpers
{
    public class OptionalToolTip
    {
        public static readonly DependencyProperty ToolTipEnabledProperty =
            DependencyProperty.RegisterAttached("ToolTipEnabled",
                                                typeof(bool),
                                                typeof(OptionalToolTip),
                                                new UIPropertyMetadata(ToolTipEnabledChanged));

        public static bool GetToolTipEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(ToolTipEnabledProperty);
        }

        public static void SetToolTipEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(ToolTipEnabledProperty, value);
        }

        private static void ToolTipEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var elem = ((FrameworkElement)d);
            if ((bool)e.NewValue)
            {
                if (elem.ToolTip != null) ((ToolTip)elem.ToolTip).Visibility = Visibility.Visible;
                else elem.ToolTip = CreateBoundToolTip(d);
            }
            else if (elem.ToolTip != null) ((ToolTip)elem.ToolTip).Visibility = Visibility.Collapsed;
        }

        private static ToolTip CreateBoundToolTip(DependencyObject d)
        {
            var tt = new ToolTip();
            var b1 = new Binding { Source = d, Path = new PropertyPath(ToolTipStyleProperty) };
            BindingOperations.SetBinding(tt, FrameworkElement.StyleProperty, b1);
            var b2 = new Binding { Source = d, Path = new PropertyPath(ToolTipContentProperty) };
            BindingOperations.SetBinding(tt, ContentControl.ContentProperty, b2);
            return tt;
        }

        public static readonly DependencyProperty ToolTipStyleProperty =
            DependencyProperty.RegisterAttached("ToolTipStyle",
                                                typeof(Style),
                                                typeof(OptionalToolTip));

        public static Style GetToolTipStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(ToolTipStyleProperty);
        }

        public static void SetToolTipStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ToolTipStyleProperty, value);
        }

        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.RegisterAttached("ToolTipContent",
                                                typeof(object),
                                                typeof(OptionalToolTip));

        public static object GetToolTipContent(DependencyObject obj)
        {
            return obj.GetValue(ToolTipContentProperty);
        }

        public static void SetToolTipContent(DependencyObject obj, object value)
        {
            obj.SetValue(ToolTipContentProperty, value);
        }
    }
}
