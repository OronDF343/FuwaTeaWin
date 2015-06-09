using System;
using System.Globalization;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class DoubleSubtractConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is double) || !(values[1] is double)) return 0d;
            return (double)values[0] - (double)values[1];
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
