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
using System.Windows.Controls;
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class PositionTimeSpanStringMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is Grid) || !(values[1] is TimeSpan)) return null;
            var pbar = (Grid)values[0];
            var dur = (TimeSpan)values[1];
            var p = NativeMethods.CorrectGetPosition(pbar);
            return TimeSpan.FromMilliseconds(dur.TotalMilliseconds / pbar.ActualWidth * p.X).ToString(@"h\:mm\:ss");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
