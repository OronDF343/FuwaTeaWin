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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FuwaTea.Lib
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class ApplicationAssociationRegistrationUI : IApplicationAssociationRegistrationUI
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void LaunchAdvancedAssociationUI(string appRegistryName);
    }

    [CoClass(typeof(ApplicationAssociationRegistrationUI))]
    [ComImport]
    [Guid("1f76a169-f994-40ac-8fc8-0959e8874710")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(ApplicationAssociationRegistrationUI))]
    public interface IApplicationAssociationRegistrationUI
    {
        void LaunchAdvancedAssociationUI([MarshalAs(UnmanagedType.LPWStr)] string appRegistryName);
    }
}
