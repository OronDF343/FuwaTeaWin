using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ModularFramework;

namespace FTWPlayer.Skins
{
    public interface ISkinManager : IUIPart
    {
        ObservableCollection<ResourceDictionary> LoadedSkins { get; }
        Dictionary<string, string> IdsToFileNames { get; }
        void LoadAllSkins(ErrorCallback ec);

        // Only this function takes normal paths!
        IEnumerable<ResourceDictionary> LoadSkinChain(IEnumerable<string> files);

        ResourceDictionary LoadSkinFromXamlFile(string path);
        ResourceDictionary LoadSkinFromXamlUri(string path);
        void LoadSkinsFromBaml(string dll);
        ResourceDictionary LoadSkinFromBaml(string dllResource);
        ResourceDictionary GetSkin(string id);

        // Implies VerifySkinChain
        List<ResourceDictionary> CreateSimpleSkinChain(string id);

        string VerifySkinChain(IEnumerable<ResourceDictionary> chain);
        IEnumerable<ResourceDictionary> GetAvailableChildSkins(string id);
        IEnumerable<ResourceDictionary> GetAvailableChildSkins(IEnumerable<ResourceDictionary> chain);
        string ExpandPath(string path);
        string ShortenPath(string path);
    }
}
