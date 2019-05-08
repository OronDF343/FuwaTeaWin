using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class JoinedListField : BasicListField, IMetadataField
    {
        private readonly uint _maxLength;

        public JoinedListField(string separator = "/", uint maxLength = 0)
        {
            Separator = separator;
            _maxLength = maxLength;
        }

        public string Separator { get; }

        public override uint MaxCount => (uint)(_maxLength / (Separator.Length + 1)) + 1;

        public override uint MaxLength => _maxLength - (uint)(Separator.Length * Math.Max(0, Value.Count - 1));

        public void ParseFrom(string s)
        {
            JoinedValue = s;
        }

        public string JoinedValue
        {
            get => ToString();
            set => Value = value.Split(new[] { Separator }, StringSplitOptions.None).ToList();
        }

        public override string ToString()
        {
            return string.Join(Separator, Value);
        }
    }
}
