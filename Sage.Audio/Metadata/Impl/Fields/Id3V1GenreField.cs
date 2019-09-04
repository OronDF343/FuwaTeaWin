using TagLib;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public class Id3V1GenreField : BasicTextField
    {
        public override uint MaxLength => 0;
        public byte Index
        {
            get => Genres.AudioToIndex(Value);
            set => Value = Genres.IndexToAudio(value);
        }
    }
}
