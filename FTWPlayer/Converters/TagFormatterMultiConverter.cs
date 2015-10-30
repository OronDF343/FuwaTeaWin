using System;
using System.Globalization;
using System.Windows.Data;
using FuwaTea.Metadata;

namespace FTWPlayer.Converters
{
    public class TagFormatterMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[1] is string)) return parameter ?? "[Null]";
            if (!(values[0] is IMusicInfoModel)) return values[1];
            var info = (IMusicInfoModel)values[0];
            var fmt = (string)values[1];
            return FormatUtils.FormatHeader(info, fmt);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
