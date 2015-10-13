using System;

namespace FuwaTea.Metadata.Tags
{
    public abstract class Tag : TagData
    {
        public abstract bool IsWriteSupported { get; }

        public abstract void SaveTags();

        public TimeSpan Duration { get; protected set; }
        public int Bitrate { get; protected set; }
    }
}
