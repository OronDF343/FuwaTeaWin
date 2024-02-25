using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as double? ?? 0;
            var p = double.Parse(parameter as string ?? "0");
            return TimeSpan.FromSeconds(Math.Max(v, p));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
