using System;
using System.Text.RegularExpressions;

namespace Sage.Audio.Metadata.Impl.Fields
{
    /// <summary>
    /// ISO 8601 subset used by ID3v2.4: yyyy[-MM[-dd[THH[:mm[:ss]]]]]
    /// </summary>
    public class Id3V24DateTimeField : BasicDateTimeField
    {
        public override byte MaxResolution => 6;

        public override void SetFrom(string str)
        {
            // I love regex
            var m = Regex.Match(str, @"^(?<y>[0-9]{4})(?:-(?<M>[0-9]{2})(?:-(?<d>[0-9]{2})(?:T(?<H>[0-9]{2})(?::(?<m>[0-9]{2})(?::(?<s>[0-9]{2}))?)?)?)?)?$");
            if (!m.Success) throw new FormatException();
            if (m.Groups["y"].Success) Year = ushort.Parse(m.Groups["y"].Value);
            if (m.Groups["M"].Success) Month = byte.Parse(m.Groups["M"].Value);
            if (m.Groups["d"].Success) Day = byte.Parse(m.Groups["d"].Value);
            if (m.Groups["H"].Success) Hour = byte.Parse(m.Groups["H"].Value);
            if (m.Groups["m"].Success) Minute = byte.Parse(m.Groups["m"].Value);
            if (m.Groups["s"].Success) Second = byte.Parse(m.Groups["s"].Value);
        }

        public override string ToString()
        {
            if (Year == null) return "";
            if (Month == null) return $"{Year:0000}";
            if (Day == null) return $"{Year:0000}-{Month:00}";
            if (Hour == null) return $"{Year:0000}-{Month:00}-{Day:00}";
            if (Minute == null) return $"{Year:0000}-{Month:00}-{Day:00}T{Hour:00}";
            if (Second == null) return $"{Year:0000}-{Month:00}-{Day:00}T{Hour:00}:{Minute:00}";
            return $"{Year:0000}-{Month:00}-{Day:00}T{Hour:00}:{Minute:00}:{Second:00}";
        }
    }
}
