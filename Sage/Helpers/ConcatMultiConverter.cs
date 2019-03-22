using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class ConcatMultiConverter : IMultiValueConverter
    {
        public static ConcatMultiConverter Instance = new ConcatMultiConverter();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var s = "";
            foreach (var o in values)
                s += o.ToString();
            return s;
        }
    }
}
