using System;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicDateTimeField : IDateTimeField
    {
        public BasicDateTimeField(byte resolution = 6)
        {
            MaxResolution = resolution;
        }

        public DateTime? Value
        {
            get => Year == null ? null : new DateTime(Year.Value, Month ?? 1, Day ?? 1, Hour ?? 0, Minute ?? 0, Second ?? 0) as DateTime?;
            set
            {
                if (value == null) return;
                var v = value.Value;
                if (MaxResolution > 0) Year = (ushort)v.Year;
                if (MaxResolution > 1) Month = (byte?)v.Month;
                if (MaxResolution > 2) Day = (byte?)v.Day;
                if (MaxResolution > 3) Hour = (byte?)v.Hour;
                if (MaxResolution > 4) Minute = (byte?)v.Minute;
                if (MaxResolution > 5) Second = (byte?)v.Second;
            }
        }

        public virtual void ParseFrom(string s)
        {
            Value = DateTime.Parse(s);
        }

        public ushort? Year { get; set; }
        public byte? Month { get; set; }
        public byte? Day { get; set; }
        public byte? Hour { get; set; }
        public byte? Minute { get; set; }
        public byte? Second { get; set; }
        
        public virtual byte MaxResolution { get; }
    }
}