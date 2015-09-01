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
using FuwaTea.Common.Models;
using FuwaTea.Data.Playlist.Tags;

namespace FuwaTea.Logic.Playlist
{
    public class MusicInfoModel : IMusicInfoModel
    {
        public Guid UniqueId { get; }
        public FileInfo FileInfo { get; }
        public string FilePath => FileInfo.FullName;
        public string FileName => FileInfo.Name;
        public string FileType => Path.GetExtension(FilePath);
        public TimeSpan Duration => Tag.Duration;
        public int Bitrate => Tag.Bitrate;
        public Tag Tag { get; }

        public MusicInfoModel(string path, Tag tag)
        {
            FileInfo = new FileInfo(path);
            Tag = tag;
            UniqueId = Guid.NewGuid();
        }
    }
}
