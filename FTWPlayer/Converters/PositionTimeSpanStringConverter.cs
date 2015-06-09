using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class PositionTimeSpanStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is Grid) || !(values[1] is TimeSpan)) return null;
            var pbar = (Grid)values[0];
            var dur = (TimeSpan)values[1];
            var p = NativeMethods.CorrectGetPosition(pbar);
            return TimeSpan.FromMilliseconds(dur.TotalMilliseconds / pbar.ActualWidth * p.X).ToString(@"h\:mm\:ss");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
