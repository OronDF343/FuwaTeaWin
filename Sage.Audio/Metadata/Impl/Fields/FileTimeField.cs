using System;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class FileTimeField : BasicDateTimeField
    {
        public override byte MaxResolution => 6;

        public override void SetFrom(string s)
        {
            FileTimeValue = long.Parse(s);
        }

        public long? FileTimeValue
        {
            get => Value?.ToFileTime();
            set => Value = value == null ? (DateTime?)null : DateTime.FromFileTime(value.Value);
        }
    }
}