namespace FuwaTea.Metadata.Tags
{
    public interface ITagProvider : IMetadataLoader
    {
        Tag Create(string path);
    }
}
