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
using LayerFramework.Exceptions;

namespace LayerFramework
{
    public static class TryUtils
    {
        public static T TryCreateInstance<T>(Type t, params object[] parameters)
        {
            return (T)TryCreateInstance(t, parameters);
        }

        public static T TryCreateInstance<T>(Type t)
        {
            return (T)TryCreateInstance(t);
        }

        public static object TryCreateInstance(Type t)
        {
            object o;
            try
            {
                o = Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                throw new ActivatorException(t, ex);
            }
            return o;
        }

        public static object TryCreateInstance(Type t, params object[] parameters)
        {
            object o;
            try
            {
                o = Activator.CreateInstance(t, parameters);
            }
            catch (Exception ex)
            {
                throw new ActivatorException(t, ex);
            }
            return o;
        }
    }
}
