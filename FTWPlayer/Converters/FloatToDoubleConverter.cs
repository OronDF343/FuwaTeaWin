using System;
using System.Globalization;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class FloatToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)(float)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (float)(double)value;
        }
    }
}
