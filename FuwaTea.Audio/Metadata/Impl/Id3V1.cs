using System.Collections.Generic;

namespace FuwaTea.Audio.Metadata.Impl
{
    public class Id3V1 : MetadataBase
    {
        public Id3V1(bool isReadOnly = false)
            : base(isReadOnly, false) { }

        public override IDictionary<string, IList<string>> ExtendedFields { get; } = null;
        public override ITextField Title { get; } = new BasicTextField(30);
        public override IListField Artist { get; } = new JoinedListField(";", 30);
        public override ITextField Album { get; } = new BasicTextField(30);
        public override IDateTimeField Year { get; } = new BasicDateTimeField(1);
        public override IListField Comment { get; } = new BasicListField(1, 28);
        public override INumericField Track { get; } = new BasicNumericField(255);
        public override IListField Genre { get; } = new Id3V1GenreField();
    }
}
