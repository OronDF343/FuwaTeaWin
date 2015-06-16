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

using System.Collections.Generic;
using System.IO;
using FuwaTea.Common.Models;
using TagLib;
using File = TagLib.File;

namespace FuwaTea.Data.Playlist.Tags
{
    [DataElement("TagLib# tag reader")]
    public class TagLibReader : ITagReader
    {
        public MusicInfoModel ReadTag(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tagFile = File.Create(new StreamFileAbstraction(path, stream, stream));
                var model = new MusicInfoModel(path, tagFile.Tag, tagFile.Properties.Duration,
                                               tagFile.Properties.AudioBitrate);
                tagFile.Dispose(); // TODO: check if there is ObjectDisposedException
                stream.Close();
                return model;
            }
        }

        public IEnumerable<string> SupportedFileTypes 
        {
            get // TODO: double-check
            {
                return new[]
                {
                    ".aac",
                    ".m4a",
                    ".aiff",
                    ".ape",
                    ".asf",
                    ".aud",
                    ".flac",
                    ".ogg",
                    ".mp3",
                    ".wav",
                    ".wv",
                    ".adts",
                    ".wma",
                    ".mka"
                };
            }
        }
    }
}
