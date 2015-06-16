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
using System.Reflection;
using FuwaTea.Lib.Exceptions;

namespace LayerFramework.Exceptions
{
    public class ProcessorArchitectureMismatchException : Exception, IDepthElement
    {
        public AssemblyName OtherAssembly { get; private set; }
        public ProcessorArchitecture CurrentArchitecture { get; private set; }
        public ProcessorArchitecture OtherArchitecture { get; private set; }

        public ProcessorArchitectureMismatchException(AssemblyName otherAssembly, ProcessorArchitecture current, ProcessorArchitecture other)
        {
            OtherAssembly = otherAssembly;
            CurrentArchitecture = current;
            OtherArchitecture = other;
        }

        public override string Message
        {
            get
            {
                return string.Format("The target platform of the assembly {0} is {1}, which is incompatible with the target platform of the current assembly, which is {2}!",
                                     OtherAssembly.FullName, Enum.GetName(typeof(ProcessorArchitecture), OtherArchitecture),
                                     Enum.GetName(typeof(ProcessorArchitecture), CurrentArchitecture));
            }
        }

        public int DisplayDepth
        {
            get { return 0; }
        }
    }
}
