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
using TagLib;

namespace FuwaTea.Data.Playlist.Tags
{
    public class MusicInfoModel : IMusicInfoModel
    {
        public Guid UniqueId { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public string FilePath { get { return FileInfo.FullName; } }
        public string FileName { get { return FileInfo.Name; } }
        public string FileType { get { return Path.GetExtension(FilePath); } }
        public TimeSpan Duration { get; private set; }
        public int Bitrate { get; private set; }
        public Tag Tag { get; private set; } // TODO: create new tag and remove taglib dependency from common

        public MusicInfoModel(string path, Tag tag, TimeSpan duration, int bitrate)
        {
            FileInfo = new FileInfo(path);
            Tag = tag;
            Duration = duration;
            Bitrate = bitrate;
            UniqueId = Guid.NewGuid();
        }
    }
}
