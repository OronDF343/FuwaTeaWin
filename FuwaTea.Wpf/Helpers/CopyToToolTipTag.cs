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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FuwaTea.Wpf.Helpers
{
    /// <summary>
    /// This class will help you bind to the parent of a tooltip. It will set the value of the tooltip's Tag property to it's parent.
    /// </summary>
    /// <remarks>This is required for instances where the Placement of the tooltip is not relative.</remarks>
    public class CopyToToolTipTag
    {
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled",
                                                typeof(bool),
                                                typeof(CopyToToolTipTag),
                                                new UIPropertyMetadata(EnabledChanged));

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        private static void EnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue) return;
            if (!(d is FrameworkElement)) return;
            var fe = (FrameworkElement)d;
            var dpd = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ToolTipProperty, d.GetType());
            if (dpd == null) return;
            if ((bool)e.NewValue) { dpd.AddValueChanged(fe, ToolTipChanged); ToolTipChanged(fe, new EventArgs()); }
            else dpd.RemoveValueChanged(fe, ToolTipChanged);
        }

        private static void ToolTipChanged(object sender, EventArgs eventArgs)
        {
            if (!(sender is FrameworkElement)) return;
            var fe = (FrameworkElement)sender;
            var tip = fe.ToolTip as ToolTip;
            if (tip != null) tip.Tag = fe;
        }
    }
}
