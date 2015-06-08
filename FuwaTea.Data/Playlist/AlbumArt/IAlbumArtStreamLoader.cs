using System.Collections.Generic;
using System.IO;

namespace FuwaTea.Data.Playlist.AlbumArt
{
    public interface IAlbumArtStreamLoader : IDataElement
    {
        Stream GetEmbeddedImage(MusicInfoModel m);
        Stream GetImageInFolder(string folder, HashSet<string> filetypeWhitelist, List<string> filenameWhitelist = null);
        Stream GetCustomImage(string pathToImage);
    }
}
