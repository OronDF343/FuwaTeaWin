using System;

namespace FuwaTea.Common.Models
{
    public class NullTag : Tag
    {
        public NullTag(int bitrate, TimeSpan duration)
        {
            Bitrate = bitrate;
            Duration = duration;
        }

        public override bool IsEmpty => false;

        public override void Clear() { }

        public override bool IsWriteSupported => false;

        public override void SaveTags() { }
    }
}
