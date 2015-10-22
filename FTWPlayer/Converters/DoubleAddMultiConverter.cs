using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class DoubleAddMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Where(o => o is double).Cast<double>().Aggregate((s, d) => s + d) + (parameter as double? ?? 0.0);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
