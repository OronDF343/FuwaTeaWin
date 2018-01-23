using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using FuwaTea.Lib.DataModel;
using JetBrains.Annotations;

namespace FTWPlayer.Converters
{
    public class FindByKeyMultiConverter<TKey> : IMultiValueConverter
    {
        [CanBeNull]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is IEnumerable<IKeyedElement<TKey>>) || !(values[1] is TKey)) return null;
            return ((IEnumerable<IKeyedElement<TKey>>)values[0]).FirstOrDefault(v => Equals(v.Key, (TKey)values[1]));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
