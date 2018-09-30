using TagLib;

namespace FuwaTea.Audio.Metadata.Impl
{
    public class Picture : IPicture
    {
        public string MimeType { get; set; }
        public PictureType PictureType { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
    }
}
