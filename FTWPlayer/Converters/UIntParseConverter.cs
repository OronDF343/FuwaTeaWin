using System;
using System.Globalization;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class UIntParseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint res;
            var s = uint.TryParse((string)value, out res);
            return s ? res : 0;
        }
    }
}
