﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class BooleanResizeSpecialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value ? new Thickness(4) : new Thickness(4, 0, 4, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
