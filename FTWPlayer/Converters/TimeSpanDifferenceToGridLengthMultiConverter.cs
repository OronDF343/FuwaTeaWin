#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class TimeSpanDifferenceToGridLengthMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter as string == "Inverse";
            if (!(values[0] is TimeSpan) || !(values[1] is TimeSpan)) return new GridLength(inverse ? 1 : 0, GridUnitType.Star);
            var pos = (TimeSpan)values[0];
            var dur = (TimeSpan)values[1];
            if (dur == TimeSpan.Zero || pos == TimeSpan.Zero) return new GridLength(inverse ? 1 : 0, GridUnitType.Star);
            var diff = dur - pos;
            if (diff.TotalMilliseconds < 0) diff = TimeSpan.Zero;
            var res = diff.TotalMilliseconds / dur.TotalMilliseconds;
            return new GridLength(inverse ? res : 1 - res, GridUnitType.Star);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
