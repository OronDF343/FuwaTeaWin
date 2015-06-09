using System.Collections.Generic;
using System.IO;
using FuwaTea.Annotations;
using FuwaTea.Common.Models;

namespace FuwaTea.Logic.Playlist.AlbumArt
{
    public interface IAlbumArtLocator : ILogicElement
    {
        [NotNull]
        AlbumArtLocations[] LocationPriority { get; set; }

        [NotNull]
        HashSet<string> ImageTypeDictionary { get; set; }

        Stream GetAlbumArt(MusicInfoModel m);

        //TODO: SetCustomImage, GetCustomImage via Cache
    }

    public enum AlbumArtLocations
    {
        Embedded, InFolder, UpFolder, DownFolder, Custom
    }
}
