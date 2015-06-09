using System;
using System.Globalization;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class UnitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: more configurable, localizable version
            var val = (float)value;
            var q = "";
            if (val >= 1000000)
            {
                q = "M";
                val /= 1000000;
            }
            else if (val >= 1000 && val < 1000000)
            {
                q = "K";
                val /= 1000;
            }
            else if (val <= 0.001 && val > 0)
            {
                q = "m";
                val *= 1000;
            }
            return val.ToString("0.###") + q + (string)parameter;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
