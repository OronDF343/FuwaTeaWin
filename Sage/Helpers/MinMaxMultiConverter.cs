using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class MinMaxMultiConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var list = values.TryCastToValueType<double>();
            var mode = (MinMaxMode)Enum.Parse(typeof(MinMaxMode), parameter as string, true);
            return mode == MinMaxMode.Min ? list.Min() :
                   mode == MinMaxMode.Max ? list.Max() : list.FirstOrDefault();
        }
    }

    public enum MinMaxMode
    {
        None,
        Min,
        Max
    }
}
