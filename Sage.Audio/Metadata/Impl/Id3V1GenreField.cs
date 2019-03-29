using System.Collections.Generic;
using System.Linq;
using TagLib;

namespace Sage.Audio.Metadata.Impl
{
    public class Id3V1GenreField : BasicListField
    {
        public override uint MaxCount => 1;
        public override uint MaxLength => 0;
        public byte Index
        {
            get => Genres.AudioToIndex(Value.FirstOrDefault());
            set => Value = new List<string>(new[] { Genres.IndexToAudio(value) });
        }
    }
}
