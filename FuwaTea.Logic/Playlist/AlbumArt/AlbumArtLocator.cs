using System.Collections.Generic;
using System.IO;
using FuwaTea.Common.Models;
using FuwaTea.Data.Playlist.AlbumArt;
using LayerFramework;

namespace FuwaTea.Logic.Playlist.AlbumArt
{
    [LogicElement("Album Art Locator")]
    public class AlbumArtLocator : IAlbumArtLocator
    {
        public AlbumArtLocator()
        {
            // TODO: C#6 properties default values
            LocationPriority = new[]
            {
                AlbumArtLocations.Custom,
                AlbumArtLocations.Embedded,
                AlbumArtLocations.InFolder,
                AlbumArtLocations.UpFolder,
                AlbumArtLocations.DownFolder
            };
            ImageTypeDictionary = new HashSet<string> { ".jpg", ".png", ".bmp" }; // TODO
        }
        public AlbumArtLocations[] LocationPriority { get; set; }
        public HashSet<string> ImageTypeDictionary { get; set; }

        public Stream GetAlbumArt(MusicInfoModel m)
        {
            //TODO: Incomplete
            Stream img = null;
            var loader = LayerFactory.GetElement<IAlbumArtStreamLoader>();
            foreach (var location in LocationPriority)
            {
                switch (location)
                {
                    case AlbumArtLocations.Embedded:
                        img = loader.GetEmbeddedImage(m);
                        break;
                    case AlbumArtLocations.InFolder:
                        img = loader.GetImageInFolder(m.FileInfo.DirectoryName, ImageTypeDictionary);
                        break;
                    case AlbumArtLocations.UpFolder:
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
