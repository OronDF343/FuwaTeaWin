using System;
using System.Collections.Generic;
using System.Text;
using TagLib;

namespace FuwaTea.Audio.Metadata
{
    public class TextField : MetadataFieldBase<string>
    {
        public TextField(uint maxCount = 0, uint maxLen = 0, Encoding encoding = null)
            : base(maxCount)
        {
            MaxLength = maxLen;
            Encoding = encoding ?? Encoding.UTF8;
        }
        
        public TextField(string value, uint maxCount = 1, uint maxLen = 0, Encoding encoding = null)
            : base(value, maxCount)
        {
            MaxLength = maxLen;
            Encoding = encoding ?? Encoding.UTF8;
        }

        public TextField(ICollection<string> value, uint maxCount = 0, uint maxLen = 0, Encoding encoding = null)
            : base(value, maxCount)
        {
            MaxLength = maxLen;
            Encoding = encoding ?? Encoding.UTF8;
        }
        public uint MaxLength { get; }
        public Encoding Encoding { get; }
    }
    public class NumericField : MetadataFieldBase<uint>
    {
        public NumericField(uint maxCount = 1, byte bits = 32)
            : base(maxCount)
        {
            Bits = bits;
        }

        public NumericField(uint value, uint maxCount = 1, byte bits = 32)
            : base(value, maxCount)
        {
            Bits = bits;
        }

        public NumericField(ICollection<uint> value, uint maxCount = 0, byte bits = 32)
            : base(value, maxCount)
        {
            Bits = bits;
        }
        
        public byte Bits { get; }
    }
    public class EnumField<T> : MetadataFieldBase<T> where T : struct, Enum
    {
        public EnumField(uint maxCount = 1)
            : base(maxCount) { }
        
        public EnumField(T value, uint maxCount = 1)
            : base(value, maxCount) { }

        public EnumField(ICollection<T> value, uint maxCount = 0)
            : base(value, maxCount) { }
    }
    public class PictureField : MetadataFieldBase<IPicture>
    {
        public PictureField(uint maxCount = 0) : base(maxCount) { }
        
        public PictureField(IPicture value, uint maxCount = 1)
            : base(value, maxCount) { }

        public PictureField(ICollection<IPicture> value, uint maxCount = 0)
            : base(value, maxCount) { }
    }
}
