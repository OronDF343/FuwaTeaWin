﻿using FuwaTea.Common.Models;

namespace FuwaTea.Data.Playlist.Tags
{
    public interface ITagProvider : IDataElement
    {
        Tag Create(string path);
    }
}