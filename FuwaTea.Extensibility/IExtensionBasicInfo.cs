using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    [InheritedExport(ExtensibilityConstants.InfoExportKey)]
    public interface IExtensionBasicInfo
    {
        string Title { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        string WebUrl { get; }
    }
}
