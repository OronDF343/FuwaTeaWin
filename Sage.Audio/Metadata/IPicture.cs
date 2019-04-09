﻿using TagLib;

namespace Sage.Audio.Metadata
{
    public interface IPicture
    {
        string MimeType { get; set; }
        PictureType PictureType { get; set; }
        string Description { get; set; }
        byte[] Data { get; set; }
    }
}