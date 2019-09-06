using System;
using System.Linq;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class JoinedListField : BasicListField
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

        public override void SetFrom(string s)
        {
            Value = s?.Split(new[] { Separator }, StringSplitOptions.None).ToList();
        }

        public string JoinedValue
        {
            get => ToString();
            set => SetFrom(value);
        }

        public override string ToString()
        {
            return Value == null ? "" : string.Join(Separator, Value);
        }
    }
}
