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

namespace Sage.Audio.Metadata
{
    [Flags]
    public enum MusicalKey : short
    {
        OffKey =    0b00_00000000,
        C =         0b00_00000001,
        D =         0b00_00000010,
        E =         0b00_00000100,
        F =         0b00_00001000,
        G =         0b00_00010000,
        A =         0b00_00100000,
        B =         0b00_01000000,
        Sharp =     0b00_10000000,
        Flat =      0b01_00000000,
        Minor =     0b10_00000000
    }
}