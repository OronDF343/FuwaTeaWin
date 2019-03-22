using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class DivideMultiConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var list = values.TryCastToValueType<double>().ToList();
            var r = list.FirstOrDefault();
            r = list.Skip(1).Aggregate(r, (current, v) => current / v);

            var p = parameter as string;
            if (!string.IsNullOrWhiteSpace(p))
            {
                var m = Regex.Match(p, @"^(\*|/)?(.+)$");
                if (!string.IsNullOrWhiteSpace(m.Groups[1].Value))
                {
                    var f = double.Parse(m.Groups[2].Value);
                    r = m.Groups[1].Value == "/" ? r / f : r * f;
                }
                else r *= double.Parse(m.Groups[0].Value);
            }

            return r;
        }
    }
}
