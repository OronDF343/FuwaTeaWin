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
using System.Collections.Generic;
using System.Windows;

namespace FuwaTea.Wpf.Helpers
{
    // Credit: NtscCobalt on StackOverflow: http://stackoverflow.com/questions/8604086/setting-a-local-implicit-style-different-from-theme-style-alternative-to-based/9506270#9506270
    public class DynamicStyle
    {
        public static Style GetBaseStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(BaseStyleProperty);
        }

        public static void SetBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(BaseStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for BaseStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseStyleProperty =
            DependencyProperty.RegisterAttached("BaseStyle", typeof(Style), typeof(DynamicStyle), new UIPropertyMetadata(StylesChanged));

        public static Style GetDerivedStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DerivedStyleProperty);
        }

        public static void SetDerivedStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DerivedStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for DerivedStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DerivedStyleProperty =
            DependencyProperty.RegisterAttached("DerivedStyle", typeof(Style), typeof(DynamicStyle), new UIPropertyMetadata(StylesChanged));

        private static void StylesChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(target.GetType()))
                throw new InvalidCastException("Target must be FrameworkElement");

            var element = (FrameworkElement)target;

            var styles = new List<Style>();

            var baseStyle = GetBaseStyle(target);

            if (baseStyle != null)
                styles.Add(baseStyle);

            var derivedStyle = GetDerivedStyle(target);

            if (derivedStyle != null)
                styles.Add(derivedStyle);

            element.Style = MergeStyles(styles);
        }

        private static Style MergeStyles(ICollection<Style> Styles)
        {
            var newStyle = new Style();

            foreach (var style in Styles)
            {
                foreach (var setter in style.Setters)
                    newStyle.Setters.Add(setter);

                foreach (var trigger in style.Triggers)
                    newStyle.Triggers.Add(trigger);
            }

            return newStyle;
        }
    }
}
