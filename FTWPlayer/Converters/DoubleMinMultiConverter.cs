using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class DoubleMinMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Where(o => o is double).Cast<double>().Min();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
