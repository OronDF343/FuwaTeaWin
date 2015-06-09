using System;
using System.Globalization;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class TimeSpanFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan)) return "";
            var ts = (TimeSpan)value;
            return ts.ToString(@"h\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return str == null ? TimeSpan.Zero : TimeSpan.ParseExact(str, @"h\:mm\:ss", null);
        }
    }
}
