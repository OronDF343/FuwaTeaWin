using System;
using System.Collections.Generic;
using System.Linq;
using FuwaTea.Lib;

namespace Sage.Audio.Metadata.Impl
{
    public class MusicalKeyField : IEnumField<MusicalKey>
    {
        public MusicalKey? Value { get; set; }

        public string StringValue
        {
            get => ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Value = null;
                    return;
                }

                if (value.Length > 3) throw new FormatException("Value too long");

                if (value == "o")
                {
                    Value = MusicalKey.OffKey;
                    return;
                }

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
                Value = v;
            }
        }

        public override string ToString()
        {
            if (Value == null) return "";

            var v = Value.Value;
            if (v == MusicalKey.OffKey) return "o";

            var r = "";
            var f = new HashSet<MusicalKey>(v.GetFlags());
            if (f.Contains(MusicalKey.Sharp)) r += "#";
            else if (f.Contains(MusicalKey.Flat)) r += "b";
            if (f.Contains(MusicalKey.Minor)) r += "m";
            f.ExceptWith(new[] { MusicalKey.Sharp, MusicalKey.Flat, MusicalKey.Minor, MusicalKey.OffKey });
            return f.FirstOrDefault() + r;
        }
    }
}