using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Lib;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class MusicalKeyField : IEnumField<MusicalKey>
    {
        public MusicalKey? Value { get; set; }
        public void ParseFrom(string s)
        {
            StringValue = s;
        }

        public string StringValue
        {
            get => FormatMusicalKey(Value) ?? "";
            set => Value = ParseMusicalKey(value);
        }

        public static MusicalKey? ParseMusicalKey(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (value.Length > 3) throw new FormatException("Value too long");

            if (value == "o")
                return MusicalKey.OffKey;

            var v = value.Substring(0, 1).ParseOrDefault<MusicalKey>();
            if (v == null) throw new FormatException("Invalid note");
            if (value.Length > 1)
            {
                switch (value[1])
                {
                    case '#':
                        v |= MusicalKey.Sharp;
                        break;
                    case 'b':
                        v |= MusicalKey.Flat;
                        break;
                    case 'm':
                        if (value.Length > 2) throw new FormatException("Invalid data after minor key specifier");
                        v |= MusicalKey.Minor;
                        break;
                    default: throw new FormatException("Invalid first modifier");
                }
            }

            if (value.Length > 2)
            {
                if (value[2] == 'm') v |= MusicalKey.Minor;
                else throw new FormatException("Invalid second modifier");
            }

            return v;
        }

        public static string FormatMusicalKey(MusicalKey? value)
        {
            if (value == null) return null;

            var v = value.Value;
            if (v == MusicalKey.OffKey) return "o";

            var r = "";
            var f = new HashSet<MusicalKey>(v.GetFlags());
            if (f.Contains(MusicalKey.Sharp)) r += "#";
            else if (f.Contains(MusicalKey.Flat)) r += "b";
            if (f.Contains(MusicalKey.Minor)) r += "m";
            f.ExceptWith(new[] { MusicalKey.Sharp, MusicalKey.Flat, MusicalKey.Minor, MusicalKey.OffKey });
            return f.FirstOrDefault() + r;
        }

        public override string ToString()
        {
            return FormatMusicalKey(Value) ?? "null";
        }
    }
}