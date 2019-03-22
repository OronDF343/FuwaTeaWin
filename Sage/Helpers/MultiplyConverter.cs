using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = !(parameter is string p) ? 0.0 : double.Parse(p);
            return (value as double? ?? 0) * d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
