using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace FTWPlayer.Converters
{
    public class NullCheckConverter : IValueConverter
    {
        public object Convert(object value, [NotNull] Type targetType, object parameter,
                              [NotNull] CultureInfo culture)
        {
            return parameter as bool? == true ? value == null : value != null;
        }

        public object ConvertBack(object value, [NotNull] Type targetType, object parameter,
                                  [NotNull] CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
