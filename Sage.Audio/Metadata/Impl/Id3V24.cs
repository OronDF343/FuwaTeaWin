namespace Sage.Audio.Metadata.Impl
{
    public class Id3V24 : Id3V23
    {
        public Id3V24(bool isReadOnly = false, bool supportsUnicode = true)
            : base(isReadOnly, supportsUnicode) { }

        public override IDateTimeField Year { get; } = new Id3V24DateTimeField();

        public override IDateTimeField EncodingTime { get; } = new Id3V24DateTimeField();

        public override ITextField Mood { get; } = new BasicTextField();

        public override IListField MusicianCredits { get; } = new BasicListField();

        public override IDateTimeField OrigReleaseTime { get; } = new Id3V24DateTimeField();

        public override IDateTimeField ReleaseTime { get; } = new Id3V24DateTimeField();

        public override ITextField SetSubtitle { get; } = new BasicTextField();

        public override IDateTimeField TaggingTime { get; } = new Id3V24DateTimeField();
    }
}
