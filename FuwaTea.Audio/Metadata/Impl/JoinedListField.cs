using System;
using System.Linq;

namespace FuwaTea.Audio.Metadata.Impl
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
