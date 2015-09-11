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
using System.Linq;
using FuwaTea.Common.Models;

namespace FuwaTea.Data.Playlist.AlbumArt
{
    [DataElement("Album Art Loader")]
    public class AlbumArtStreamLoader : IAlbumArtStreamLoader
    {
        public Stream GetEmbeddedImage(IMusicInfoModel m)
        {
            var img = m.Tag?.Pictures?.FirstOrDefault(p => p.Type == PictureType.FrontCover);
            return img == default(IPicture) ? null : new MemoryStream(img.Data, false);
        }

        public Stream GetImageInFolder(string folder, HashSet<string> filetypeWhitelist, List<string> filenameWhitelist = null)
        {
            // TODO: handle exceptions
            var f = new DirectoryInfo(folder);
            FileInfo[] imgs = null;
            if (filenameWhitelist == null) imgs = f.GetFiles();
            else
            {
                while(filenameWhitelist.Count > 0)
                {
                    imgs = f.GetFiles(filenameWhitelist[0]);
                    if (imgs.Length > 0) break;
                    filenameWhitelist.RemoveAt(0);
                }
            }
            if (imgs == null || imgs.Length == 0) return null;
            var img = imgs.FirstOrDefault(i => filetypeWhitelist.Contains(i.Extension));
            return img == null ? null : new FileStream(img.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream GetCustomImage(string pathToImage)
        {
            return new FileStream(pathToImage, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public IEnumerable<string> SupportedFileTypes => null;
        // TODO: Not Applicable?
    }
}
