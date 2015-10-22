using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class ScaleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return 0.0;
            if (values.Take(3).Any(o => !(o is double))) return values[0] as double? ?? 0.0;
            // min + (max - min) * scale
            return (double)values[0] + ((double)values[1] - (double)values[0]) * (double)values[2];
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
