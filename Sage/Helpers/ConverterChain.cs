using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Metadata;

namespace Sage.Helpers
{
    public class ConverterChain : IValueConverter, IMultiValueConverter
    {
        /// <summary>
        /// Gets the collection of child converters.
        /// </summary>
        [Content]
        public IList<IValueConverter> Converters { get; set; } = new List<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
        }

        public IMultiValueConverter MultiValueConverter { get; set; }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (MultiValueConverter == null) throw new InvalidOperationException("MultiValueConverter is required");
            return Convert(MultiValueConverter.Convert(values, targetType, parameter, culture), targetType, parameter, culture);
        }
    }

    public class ConverterWithParameter : IValueConverter
    {
        public IValueConverter Converter { get; set; }
        public object Parameter { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converter.Convert(value, targetType, Parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converter.ConvertBack(value, targetType, Parameter, culture);
        }
    }

    public class MultiConverterWithParameter : IMultiValueConverter
    {
        public IMultiValueConverter Converter { get; set; }
        public object Parameter { get; set; }
        
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            return Converter.Convert(values, targetType, Parameter, culture);
        }
    }
}
