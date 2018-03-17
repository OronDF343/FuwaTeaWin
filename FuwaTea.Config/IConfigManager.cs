using System.ComponentModel.Composition;

namespace FuwaTea.Config
{
    [InheritedExport]
    public interface IConfigManager
    {
        void LoadAllConfigPages();
        void SaveAllConfigPages();

        IConfigPage GetPage(string key);
        IConfigPageMetadata GetPageMetadata(string key);
        IConfigPage this[string key] { get; }
    }
}
