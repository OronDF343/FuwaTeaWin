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
using System.IO;
using TagLib;

namespace FuwaTea.Common.Models
{
    public interface IMusicInfoModel
    {
        Guid UniqueId { get; }
        FileInfo FileInfo { get; }
        string FilePath { get; }
        string FileName { get; }
        string FileType { get; }
        TimeSpan Duration { get; }
        int Bitrate { get; }
        Tag Tag { get; } // TODO: create new tag and remove taglib dependency from common
    }
}
