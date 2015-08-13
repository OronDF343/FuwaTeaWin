using System;
using FuwaTea.Common.Models;

namespace FuwaTea.Data.Playlist.Tags
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
            throw new NotImplementedException();
        }

        public byte[] Data { get; }
        public string Description { get; }
        public string MimeType { get; }
        public PictureType Type { get; }
    }
}
