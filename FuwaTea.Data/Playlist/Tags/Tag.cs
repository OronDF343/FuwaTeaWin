using System;
using FuwaTea.Common.Models;

namespace FuwaTea.Data.Playlist.Tags
{
    public abstract class Tag : TagData
    {
        public abstract bool IsWriteSupported { get; }

        public abstract void SaveTags();

        public TimeSpan Duration { get; protected set; }
        public int Bitrate { get; protected set; }
    }
}
