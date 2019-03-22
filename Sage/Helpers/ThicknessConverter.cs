using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Data.Converters;

namespace Sage.Helpers
{
    public class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = parameter as string ?? "All";
            var v = value as double? ?? 0;
            var m = Regex.Match(p, @"^(.+?)(?:(\*|/)(.+))?$");
            var l = (Side)Enum.Parse( typeof(Side), m.Groups[1].Value, true);
            if (!string.IsNullOrWhiteSpace(m.Groups[2].Value))
            {
                var f = double.Parse(m.Groups[3].Value);
                v = m.Groups[2].Value == "/" ? v / f : v * f;
            }
            
            Thickness t;
            switch (l)
            {
                case Side.Left:
                    t = new Thickness(v,0,0,0);
                    break;
                case Side.Top:
                    t = new Thickness(0,v,0,0);
                    break;
                case Side.Right:
                    t = new Thickness(0,0,v,0);
                    break;
                case Side.Bottom:
                    t = new Thickness(0,0,0,v);
                    break;
                case Side.Horizontal:
                    t = new Thickness(v,0);
                    break;
                case Side.Vertical:
                    t = new Thickness(0,v);
                    break;
                case Side.All:
                    t = new Thickness(v);
                    break;
                default:
                    t = new Thickness(0);
                    break;
            }
            return t;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum Side
    {
        None   = 0b0000,
        Left   = 0b0001,
        Top    = 0b0010,
        Right  = 0b0100,
        Bottom = 0b1000,
        Horizontal = Left | Right,
        Vertical = Top | Bottom,
        All = Horizontal | Vertical
    }
}
