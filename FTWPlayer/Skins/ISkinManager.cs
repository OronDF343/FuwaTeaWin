using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModularFramework;

namespace FTWPlayer.Skins
{
    public interface ISkinManager : IUIPart
    {
        ObservableCollection<SkinPackage> LoadedSkins { get; }
        SkinPackage GetLoadedSkin(string source);
        void LoadAllSkins(ErrorCallback ec);

        // Only this function takes shortened paths! It also adds missing ResourceDictionaries from the default skin
        SkinPackage LoadSkin(string source, HashSet<string> children = null);
        SkinPackage LoadFallbackSkin();
        
        string ExpandPath(string path);
        string ShortenPath(string path);
    }
}
