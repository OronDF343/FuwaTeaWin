using System;
using System.Globalization;
using System.Windows.Data;
using FuwaTea.Metadata;
using FuwaTea.Metadata.FormatUtils;

namespace FTWPlayer.Converters
{
    public class TagFormatterMultiConverter : IMultiValueConverter
    {
        private readonly FormatParser _parser = new FormatParser();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[1] is string)) return parameter ?? "[Null]"; // TODO: Localize
            if (!(values[0] is IMusicInfoModel)) return values[1];
            var info = (IMusicInfoModel)values[0];
            var fmt = (string)values[1];
            return _parser.FormatObject(fmt, info);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
