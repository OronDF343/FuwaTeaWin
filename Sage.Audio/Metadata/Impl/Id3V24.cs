using Sage.Audio.Metadata.Impl.Fields;

namespace Sage.Audio.Metadata.Impl
{
    public class Id3V24 : Id3V23
    {
        public bool CompatibilityMode { get; }

        public Id3V24(bool isReadOnly = false, bool supportsUnicode = true, bool compatV3 = false)
            : base(isReadOnly, supportsUnicode)
        {
            FieldsById.Add(CommonFieldIds.EncodingTime, new Id3V24DateTimeField());
            FieldsById.Add(CommonFieldIds.Mood, new BasicTextField());
            FieldsById.Add(CommonFieldIds.MusicianCredits, new BasicListField());
            FieldsById.Add(CommonFieldIds.ReleaseTime, new Id3V24DateTimeField());
            FieldsById.Add(CommonFieldIds.SetSubtitle, new BasicTextField());
            FieldsById.Add(CommonFieldIds.TaggingTime, new Id3V24DateTimeField());
            
            CompatibilityMode = compatV3;
            if (!CompatibilityMode)
            {
                FieldsById[CommonFieldIds.Artist] = new BasicListField();
                FieldsById[CommonFieldIds.Year] = new Id3V24DateTimeField();
                FieldsById[CommonFieldIds.Year] = new Id3V24DateTimeField();
                FieldsById[CommonFieldIds.OrigReleaseTime] = new Id3V24DateTimeField();
            }
        }
    }
}
