using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;
using Sage.Audio.Metadata;
using Sage.Lib.MetaText;

namespace FTWPlayer.Converters
{
    public class TagFormatterMultiConverter : IMultiValueConverter
    {
        private readonly FormatParser _parser = new FormatParser();
        public object Convert([NotNull] object[] values, [NotNull] Type targetType, [CanBeNull] object parameter,
                              [NotNull] CultureInfo culture)
        {
            if (!(values[1] is string)) return parameter ?? "[Null]"; // TODO: Localize
            if (!(values[0] is IMetadata)) return values[1];
            var info = (IMetadata)values[0];
            var fmt = (string)values[1];
            return _parser.FormatObject(fmt, info);
        }
        public object[] ConvertBack([CanBeNull] object value, [NotNull] Type[] targetTypes, [CanBeNull] object parameter,
                                    [NotNull] CultureInfo culture) { throw new NotImplementedException(); }
    }
}
