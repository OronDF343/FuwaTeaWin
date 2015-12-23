using System;
using System.Globalization;
using System.Windows.Data;
using ModularFramework;

namespace FTWPlayer.Converters
{
    public class NullCheckConverter : IValueConverter
    {
        public object Convert([CanBeNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter,
                              [NotNull] CultureInfo culture)
        {
            return parameter as bool? == true ? value == null : value != null;
        }

        public object ConvertBack([CanBeNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter,
                                  [NotNull] CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
