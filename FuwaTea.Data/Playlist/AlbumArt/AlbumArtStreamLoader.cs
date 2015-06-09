using System.Collections.Generic;
using System.IO;
using System.Linq;
using FuwaTea.Common.Models;
using TagLib;

namespace FuwaTea.Data.Playlist.AlbumArt
{
    [DataElement("Album Art Loader")]
    public class AlbumArtStreamLoader : IAlbumArtStreamLoader
    {
        public Stream GetEmbeddedImage(MusicInfoModel m)
        {
            var img = m.Tag.Pictures.FirstOrDefault(p => p.Type == PictureType.FrontCover);
            return img == default(IPicture) ? null : new MemoryStream(img.Data.Data, false);
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

        public IEnumerable<string> SupportedFileTypes { get { return null; } } // TODO: Not Applicable?
    }
}
