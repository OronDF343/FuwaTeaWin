﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class BasicDateTimeField : MetadataField, IDateTimeField
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
                if (MaxResolution > 0) Year = (ushort?)value?.Year;
                if (MaxResolution > 1) Month = (byte?)value?.Month;
                if (MaxResolution > 2) Day = (byte?)value?.Day;
                if (MaxResolution > 3) Hour = (byte?)value?.Hour;
                if (MaxResolution > 4) Minute = (byte?)value?.Minute;
                if (MaxResolution > 5) Second = (byte?)value?.Second;
            }
        }

        public override void SetFrom(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                Value = null;
            else if (DateTime.TryParse(s, out var dt))
                Value = dt;
            else if (s.Length == 4 && uint.TryParse(s, out var y))
                SetFrom(y);
            else
                Value = null;
        }

        public override void SetFrom(uint s)
        {
            Value = new DateTime((int)s, 1, 1);
        }

        public override void SetFrom(IEnumerable<string> s)
        {
            SetFrom(s.FirstOrDefault());
        }

        public override string ToString()
        {
            return Value?.ToString("s") ?? "";
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