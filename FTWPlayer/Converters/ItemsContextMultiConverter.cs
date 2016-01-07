using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class ItemsContextMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Length == 2 ? values.ToArray() : null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
