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
using System.Windows.Data;

namespace FTWPlayer.Converters
{
    public class UnitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: more configurable, localizable version
            var val = (float)value;
            var q = "";
            if (val >= 1000000)
            {
                q = "M";
                val /= 1000000;
            }
            else if (val >= 1000 && val < 1000000)
            {
                q = "K";
                val /= 1000;
            }
            else if (val <= 0.001 && val > 0)
            {
                q = "m";
                val *= 1000;
            }
            return val.ToString("0.###") + q + (string)parameter;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
