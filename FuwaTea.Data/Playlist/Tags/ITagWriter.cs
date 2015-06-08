using TagLib;

namespace FuwaTea.Data.Playlist.Tags
{
    public interface ITagWriter : IDataElement
    {
        void WriteTag(Tag tag, string path);
    }
}
