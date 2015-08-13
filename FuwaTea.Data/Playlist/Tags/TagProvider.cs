using System;
using System.Collections.Generic;
using FuwaTea.Common.Models;

namespace FuwaTea.Data.Playlist.Tags
{
    public abstract class TagProvider : TagData, IDataElement
    {
        public abstract bool IsWriteSupported { get; }

        public abstract void OpenFile(string path);

        public abstract void SaveTags();

        public abstract IEnumerable<string> SupportedFileTypes { get; }

        public TimeSpan Duration { get; protected set; }
        public int Bitrate { get; protected set; }
    }
}
