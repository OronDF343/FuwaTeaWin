using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class RiffInfo : MetadataBase
    {
        public RiffInfo(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode) { }

        // TODO: These are supported by TagLib#. Determine if there are others we can add support for.
        // TODO: Custom tags technically supported, but only with 4-character alphanumeric IDs

        public override ITextField Title { get; } = new BasicTextField();
        public override IListField Artist { get; } = new BasicListField();
        public override ITextField AlbumArtist { get; } = new BasicTextField();
        public override IDateTimeField Year { get; } = new BasicDateTimeField(1);
        public override INumericField Track { get; } = new BasicNumericField();
        public override INumericField TrackCount { get; } = new BasicNumericField();
        public override IListField Genre { get; } = new BasicListField();
        public override IListField Comment { get; } = new BasicListField();
        public override IListField Composer { get; } = new BasicListField();
        public override ITextField Copyright { get; } = new BasicTextField();
    }
}
