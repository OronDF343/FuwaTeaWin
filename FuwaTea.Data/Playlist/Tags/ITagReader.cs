namespace FuwaTea.Data.Playlist.Tags
{
    public interface ITagReader : IDataElement
    {
        MusicInfoModel ReadTag(string path);
    }
}
