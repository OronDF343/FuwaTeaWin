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
using FuwaTea.Data.Playlist.AlbumArt;
using ModularFramework;

namespace FuwaTea.Logic.Playlist.AlbumArt
{
    [LogicElement("Album Art Locator")]
    public class AlbumArtLocator : IAlbumArtLocator
    {
        public AlbumArtLocations[] LocationPriority { get; set; } =
        {
            AlbumArtLocations.Custom,
            AlbumArtLocations.Embedded,
            AlbumArtLocations.InFolder,
            AlbumArtLocations.UpFolder,
            AlbumArtLocations.DownFolder
        };
        public HashSet<string> ImageTypeDictionary { get; set; } = new HashSet<string> { ".jpg", ".png", ".bmp" };

        public Stream GetAlbumArt(IMusicInfoModel m)
        {
            //TODO: Incomplete
            Stream img = null;
            var loader = ModuleFactory.GetElement<IAlbumArtStreamLoader>();
            foreach (var location in LocationPriority)
            {
                switch (location)
                {
                    case AlbumArtLocations.Embedded:
                        img = loader.GetEmbeddedImage(m);
                        break;
                    case AlbumArtLocations.InFolder:
                        if (m.FileInfo != null)
                            img = loader.GetImageInFolder(m.FileInfo?.DirectoryName, ImageTypeDictionary);
                        break;
                    case AlbumArtLocations.UpFolder:
                        if (m.FileInfo?.Directory?.Parent != null)
                            img = loader.GetImageInFolder(m.FileInfo.Directory.Parent.FullName, ImageTypeDictionary);
                        break;
                    case AlbumArtLocations.DownFolder:
                        // TODO: implement logic
                        break;
                    case AlbumArtLocations.Custom:
                        // TODO: cached value
                        break;
                }
                if (img != null) break;
            }
            return img;
        }
    }
}
