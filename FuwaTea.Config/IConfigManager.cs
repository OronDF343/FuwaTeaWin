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

        // Statically-typed variants
        string SerializePage<T>(T page) where T : IConfigPage;
        T DeserializePage<T>(T page, string data) where T : IConfigPage;

        // Dynamically-typed variants
        string SerializePage(IConfigPage page);
        IConfigPage DeserializePage(IConfigPage page, string data);
    }
}
