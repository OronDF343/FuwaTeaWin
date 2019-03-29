using System;

namespace Sage.Audio.Metadata.Impl
{
    public class FileTimeField : BasicDateTimeField
    {
        public override byte MaxResolution => 6;

        public long? FileTimeValue
        {
            get => Value?.ToFileTime();
            set => Value = value == null ? (DateTime?)null : DateTime.FromFileTime(value.Value);
        }
    }
}