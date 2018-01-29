using System.ComponentModel.Composition;

namespace FuwaTea.Metadata.Tags
{
    [InheritedExport]
    public interface ITagProvider : IMetadataLoader
    {
        Tag Create(string path);
    }
}
