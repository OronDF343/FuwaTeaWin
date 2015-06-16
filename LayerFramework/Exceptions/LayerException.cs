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
using FuwaTea.Lib.Exceptions;

namespace LayerFramework.Exceptions
{
    public abstract class LayerException : Exception, IDepthElement
    {
        public LayerException()
        {
            DisplayDepth = 0;
        }

        public LayerException(string message)
            : base(message)
        {
            DisplayDepth = 0;
        }

        public LayerException(Exception innerException, int fallbackDisplayDepth = 1)
            : this("", innerException, fallbackDisplayDepth) { }

        public LayerException(string message, Exception innerException, int fallbackDisplayDepth = 1)
            : base(message, innerException)
        {
            DisplayDepth = innerException is IDepthElement ? ((IDepthElement)innerException).DisplayDepth + 1 : fallbackDisplayDepth;
        }

        // TODO: When C# 6.0 is released, add initializer = 0 and remove the assignment from the first two constructors.
        /// <summary>
        /// How many layers of Inner Exceptions contain useful info?
        /// </summary>
        public int DisplayDepth { get; private set; }

        public abstract string LayerName { get; }
    }
}
