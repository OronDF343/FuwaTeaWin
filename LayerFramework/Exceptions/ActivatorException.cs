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
    public class ActivatorException : Exception, IDepthElement
    {
        public Type TryType { get; private set; }

        public ActivatorException(Type tryType, Exception innerException)
            : base("", innerException)
        {
            TryType = tryType;
        }

        public override string Message
        {
            get { return string.Format("Failed to create an instance of the type {0}!", TryType.FullName); }
        }

        public int DisplayDepth
        {
            get { return InnerException is IDepthElement ? ((IDepthElement)InnerException).DisplayDepth + 1 : 1; }
        }
    }
}
