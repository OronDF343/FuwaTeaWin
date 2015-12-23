using System.IO;

namespace FuwaTea.Metadata.Tags
{
    public class Picture : IPicture
    {
        public Picture(byte[] data, string description, string mimeType, PictureType type)
        {
            Data = data;
            Description = description;
            MimeType = mimeType;
            Type = type;
        }

        public Picture(string file, string description, PictureType type)
        {
            switch (Path.GetExtension(file).ToLowerInvariant())
            {
                case ".gif":
                    MimeType = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    MimeType = "image/jpeg";
                    break;
                case ".tif":
                case ".tiff":
                    MimeType = "image/tiff";
                    break;
                case ".png":
                    MimeType = "image/png";
                    break;
                case ".bmp":
                    MimeType = "image/bmp";
                    break;
            }
            Description = description;
            Type = type;
            Data = File.ReadAllBytes(file);
        }

        public byte[] Data { get; }
        public string Description { get; }
        public string MimeType { get; }
        public PictureType Type { get; }
    }
}
