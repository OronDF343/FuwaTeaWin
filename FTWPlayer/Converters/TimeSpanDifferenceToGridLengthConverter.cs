using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class TimeSpanDifferenceToGridLengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter as string == "Inverse";
            if (!(values[0] is TimeSpan) || !(values[1] is TimeSpan)) return new GridLength(inverse ? 1 : 0, GridUnitType.Star);
            var pos = (TimeSpan)values[0];
            var dur = (TimeSpan)values[1];
            if (dur == TimeSpan.Zero || pos == TimeSpan.Zero) return new GridLength(inverse ? 1 : 0, GridUnitType.Star);
            var diff = dur - pos;
            if (diff.TotalMilliseconds < 0) diff = TimeSpan.Zero;
            var res = diff.TotalMilliseconds / dur.TotalMilliseconds;
            return new GridLength(inverse ? res : 1 - res, GridUnitType.Star);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
